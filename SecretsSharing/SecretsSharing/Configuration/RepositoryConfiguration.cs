using SecretsSharing.Repository.Repositories;
using SecretsSharing.Repository.Repositories.Interfaces;

namespace SecretsSharing.Configuration
{
    public static class RepositoryConfiguration
    {
        public static IServiceCollection AddRepositoryConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IUploadRepository, UploadRepository>();

            return services;
        }
    }
}
