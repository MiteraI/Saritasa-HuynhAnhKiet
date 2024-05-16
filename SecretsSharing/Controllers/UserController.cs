using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Domain.Entities;
using SecretsSharing.Dto.Auth;
using SecretsSharing.Service.Exceptions;
using SecretsSharing.Service.Services.Interfaces;

namespace SecretsSharing.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _log;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> log ,IUserService userService)
        {
            _log = log;
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
            try
            {
                var registeredUser = await _userService.Register(user, registerDto.Password);
                _log.LogInformation($"User {registeredUser.Email} registered");
                return Ok($"{registeredUser.Email} registered successfully");
            } catch (EmailAlreadyUsedException ex)
            {
                return BadRequest(ex.Message);
            } catch (Exception ex)
            {
                _log.LogError(ex, "Error while registering user");
                return StatusCode(500, "Error while registering user");
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.Authenticate(loginDto.Email, loginDto.Password);
                if (token == null)
                {
                    return Unauthorized();
                }

                return Ok(token);
            } catch (EmailNotFoundException ex)
            {
                return BadRequest(ex.Message);
            } catch (InvalidPasswordException ex)
            {
                return BadRequest(ex.Message);
            } catch (Exception ex)
            {
                _log.LogError(ex, "Error while authenticating user");
                return StatusCode(500, "Error while authenticating user");
            }
        }
    }
}
