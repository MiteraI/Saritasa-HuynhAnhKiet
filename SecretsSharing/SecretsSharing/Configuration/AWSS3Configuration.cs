using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;

namespace SecretsSharing.Configuration
{
    public static class AWSS3Configuration
    {
        public static IServiceCollection AddAWSS3Configuration(this IServiceCollection services, IConfiguration configuration)
        {
            var awsSettings = configuration.GetSection("AWS");
            AWSOptions awsOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(awsSettings["AccessKey"], awsSettings["SecretKey"]),
                Region = RegionEndpoint.APSoutheast1
            };
            services.AddDefaultAWSOptions(awsOptions);

            services.AddAWSService<IAmazonS3>(ServiceLifetime.Singleton);

            return services;
        }
    }
}
