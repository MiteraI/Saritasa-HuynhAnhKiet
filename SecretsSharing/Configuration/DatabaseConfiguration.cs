using Microsoft.EntityFrameworkCore;
using SecretsSharing.Domain.Entities;

namespace SecretsSharing.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<DbContext>(provider => provider.GetService<ApplicationDatabaseContext>());

            return services;
        }
    }
}
