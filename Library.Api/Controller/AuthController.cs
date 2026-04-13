using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 using Library.Application.DTOs;
using Library.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace Library.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                // Validate passwords match
                if (registerDto.Password != registerDto.ConfirmPassword)
                    return BadRequest(new { message = "Passwords do not match" });

                // Check if email already exists
                if (await _authService.EmailExistsAsync(registerDto.Email))
                    return BadRequest(new { message = "Email already registered" });

                // Register user
                var user = await _authService.RegisterAsync(
                    registerDto.Email,
                    registerDto.Password,
                    registerDto.FirstName,
                    registerDto.LastName
                );

                return Ok(new { message = "User registered successfully", email = user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", registerDto.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Login")]
        
        public async Task<IActionResult> Login(LoginDto loginDto )
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

                // Get user details for response
                var user = await _authService.GetUserByEmailAsync(loginDto.Email);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Email = user?.Email ?? string.Empty,
                    FirstName = user?.FirstName ?? string.Empty,
                    LastName = user?.LastName ?? string.Empty,
                    Expiration = DateTime.UtcNow.AddHours(1)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user {Email}", loginDto.Email);
                return Unauthorized(new { message = ex.Message });
            }
        }

    }


}

