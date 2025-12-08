using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StudentHub.Infrastructure.Identity
{
    public class AppIdentityDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
    {
        public AppIdentityDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../StudentHub.Api");
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("PgSql");

            var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppIdentityDbContext(optionsBuilder.Options);
        }
    }
}
