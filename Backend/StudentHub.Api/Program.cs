using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentHub.Api.Extensions;
using StudentHub.Api.Middlewares;
using StudentHub.Api.WebServices;
using StudentHub.Application.Interfaces.Repositories;
using StudentHub.Application.Interfaces.Services;
using StudentHub.Application.Services;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Identity;
using StudentHub.Infrastructure.Repositories;
using StudentHub.Infrastructure.Services;
using System.Net.Mime;
using System.Text.Json;

namespace StudentHub.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Serilog configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs\\", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Building application...");
                var builder = WebApplication.CreateBuilder(args);

                builder.Configuration
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();

                builder.Services.AddSerilog();

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PgSql")));

                builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("PgSql")));

                builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>()
                    .AddErrorDescriber<CustomIdentityErrorDescriberRu>() //локализация ошибок identity
                    .AddDefaultTokenProviders();

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                    options.LoginPath = "/api/Account/Login";
                    options.AccessDeniedPath = "/api/Account/Login";
                    options.SlidingExpiration = true;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    };
                });

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
                });

                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IPostRepository, PostRepository>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IPostService, PostService>();
                builder.Services.AddScoped<IFileStorageService, FileStorage>();
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();
                try
                {
                    await DbSeeder.SeedAdmin(app.Services);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex.Message);
                }

                app.UseSerilogRequestLogging();

                if (builder.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                        options.RoutePrefix = string.Empty;
                    });
                }
                if (builder.Environment.IsProduction())
                {
                    app.Urls.Add("http://0.0.0.0:5000");
                }

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseCors("AllowAll");
                app.MapControllers();

                //app.UseMiddleware<ValidationErrorMiddleware>();
               
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
            finally
            {
                Log.Information("Program shutdown");
            }
        }
    }
}
