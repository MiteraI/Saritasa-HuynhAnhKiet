using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Domain.Constants;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Dto.Auth;
using SecretsSharing.Service.Services.Interfaces;

namespace SecretsSharing.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
       private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            var registeredUser = await _userService.Register(user, registerDto.Password);
            return Ok(registeredUser);
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _userService.Authenticate(loginDto.Email, loginDto.Password);
            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
