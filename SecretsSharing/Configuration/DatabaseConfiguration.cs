using Microsoft.EntityFrameworkCore;
using SecretsSharing.Domain.Entities;
using Serilog;

namespace SecretsSharing.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Get the database URL from the environment variables for containerized deployments
            string databaseUrl = configuration.GetValue<string>("DATABASE_URL");
            if (string.IsNullOrEmpty(databaseUrl))
            {
                // If no environment is set, use the default connection string in appsettings.json
                Log.Information(configuration.GetConnectionString("DefaultConnection"));
                services.AddDbContext<ApplicationDatabaseContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });
            } else
            {
                Log.Information(databaseUrl);
                services.AddDbContext<ApplicationDatabaseContext>(options =>
                {
                    options.UseSqlServer(databaseUrl);
                });
            }
  
            // Create scope to migrate database
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
                context.Database.Migrate();
            }

            return services;
        }
    }
}
