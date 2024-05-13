using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.NameIdentifier;
            })
                .AddEntityFrameworkStores<ApplicationDatabaseContext>()
                .AddUserStore<UserStore<User, Role, ApplicationDatabaseContext, string, IdentityUserClaim<string>,
                UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
                .AddRoleStore<RoleStore<Role, ApplicationDatabaseContext, string, UserRole, IdentityRoleClaim<string>>
                >()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Jwt:Key"]!)),
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
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
