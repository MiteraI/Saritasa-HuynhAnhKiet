using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;

namespace SecretsSharing.Configuration
{
    public static class AWSS3Configuration
    {
        public static IServiceCollection AddAWSS3Configuration(this IServiceCollection services, IConfiguration configuration)
        {
            var awsEnvAccessKey = configuration.GetValue<string>("AWS_ACCESS_KEY");
            var awsEnvSecretKey = configuration.GetValue<string>("AWS_SECRET_KEY");
            // Get AWS settings from environment variables
            if (string.IsNullOrEmpty(awsEnvAccessKey) && string.IsNullOrEmpty(awsEnvSecretKey))
            {
                AWSOptions awsOptions = new AWSOptions
                {
                    Credentials = new BasicAWSCredentials(awsEnvAccessKey, awsEnvSecretKey),
                    Region = RegionEndpoint.APSoutheast1
                };
                services.AddDefaultAWSOptions(awsOptions);
            }
            else
            {
                // Get AWS settings from appsettings.json
                var awsSettings = configuration.GetSection("AWS");
                AWSOptions awsOptions = new AWSOptions
                {
                    Credentials = new BasicAWSCredentials(awsSettings["AccessKey"], awsSettings["SecretKey"]),
                    Region = RegionEndpoint.APSoutheast1
                };
                services.AddDefaultAWSOptions(awsOptions);
            }


            services.AddAWSService<IAmazonS3>(ServiceLifetime.Singleton);

            return services;
        }
    }
}
