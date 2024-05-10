using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SecretsSharing.Domain.Entities;
using System.Security.Claims;
using System.Text;

namespace SecretsSharing.Configuration
{
    public static class SecurityConfiguration
    {
        public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.NameIdentifier;
            })
                .AddEntityFrameworkStores<ApplicationDatabaseContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                }
                );
            return services;
        }

        public static IApplicationBuilder UseSecurityConfiguration(this IApplicationBuilder app)
        {
            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowCredentials();
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
