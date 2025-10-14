using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentHub.Application.Interfaces;
using StudentHub.Application.Services;
using StudentHub.Infrastructure.Data;
using StudentHub.Infrastructure.Identity;
using StudentHub.Infrastructure.Repositories;
using StudentHub.Web.WebServices;

namespace StudentHub.Web
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

                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<DbSeeder>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IPostRepository, PostRepository>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                app.UseSerilogRequestLogging();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    //try
                    //{
                    //    // Database migration
                    //    var context = services.GetRequiredService<AppDbContext>();
                    //    await context.Database.MigrateAsync();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.Error(ex.Message);
                    //}

                    try
                    {
                        // Database seeding
                        var seeder = services.GetRequiredService<DbSeeder>();
                        await seeder.SeedAdminAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message);
                    }
                }

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

                app.MapControllers();

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
