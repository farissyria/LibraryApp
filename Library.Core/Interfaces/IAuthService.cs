using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Interfaces
{
    public interface IAuthService
    {
        // Register new user
        Task<User> RegisterAsync(string email, string password, string firstName, string lastName);

        // Login user and return JWT token
        Task<string> LoginAsync(string email, string password);

        // Get user by email
        Task<User?> GetUserByEmailAsync(string email);

        // Check if email already exists
        Task<bool> EmailExistsAsync(string email);

    }
}
