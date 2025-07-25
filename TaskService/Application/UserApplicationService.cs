using Microsoft.IdentityModel.Tokens;
using Shared.Core.Entities;
using Shared.Core.Enums;
using TaskService.Application.DTOs;
using TaskService.Infrastructure.Data;

namespace TaskService.Application
{
    public class UserApplicationService : IUserApplicationService
    {

        private readonly IUserRepository _userRepository;
        public UserApplicationService(IUserRepository userRepository) => _userRepository = userRepository;

        public async Task<string> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if(user == null || BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) 
                throw new UnauthorizedAccessException("Invalid email or password");
            return "JWT TOKEN"; // TODO: Generate JWT token
        }

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Email and password are required");

            if (await _userRepository.GetByEmailAsync(dto.Email) != null)
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Client
            };

            await _userRepository.AddAsync(user);
            return "JWT TOKEN"; //TODO: Generate JWT token
        }
    }
}
