using Microsoft.AspNetCore.Server.Kestrel.Core;
using SecretsSharing.Service.Services;
using SecretsSharing.Service.Services.Interfaces;

namespace SecretsSharing.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUploadService, UploadService>();

            return services;
        }
    }
}
