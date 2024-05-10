using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecretsSharing.Domain.Constants;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Dto.Auth;
using SecretsSharing.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecretsSharing.Service.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly RoleManager<Role> _roleManager;
        protected readonly IConfiguration _configuration;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationToken> Authenticate(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var key = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds
                );


                return new AuthenticationToken
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }

            return null;
        }

        public async Task<User> Register(User user, string password)
        {
            await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, RolesConstants.USER);

            return await _userManager.FindByEmailAsync(user.Email);
        }

        private async Task<User> GetUserWithUserRolesByEmail(string email)
        {
            return await _userManager.Users
                .Include(it => it.UserRoles)
                .ThenInclude(it => it.Role)
                .SingleOrDefaultAsync(it => it.Email == email);
        }
    }
}
