using Library.Core.Entities;
using Library.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
          UserManager<User> userManager,
          SignInManager<User> signInManager,
          IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        private string GenerateJwtToken(User user)
        {
            // Get JWT settings from configuration
            var jwtKey = _configuration["Jwt:Key"] ?? "your-super-secret-key-with-at-least-32-characters-long";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "LibraryAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "LibraryClient";

            // Create claims (user information embedded in token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Create signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid email or password");

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password");

            // Generate and return JWT token
            return GenerateJwtToken(user);

        }

        public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName)
        {

            // Create new user
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Create user in database with hashed password
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            return user;
            }
    }
}

