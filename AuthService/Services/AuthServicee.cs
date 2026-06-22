using SmartBank.Authentication.DTOs;
using SmartBank.Authentication.Models;
using SmartBank.Authentication.Repositories;
using SmartBank.Authentication.Helpers;

namespace SmartBank.Authentication.Services
{
    public class AuthServicee : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;


        public AuthServicee(
            IUserRepository userRepository,
            IJwtService jwtService,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtService = jwtService;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            var existingUser =
                await _userRepository.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return "Email already exists.";
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(request.Password),

                RoleId = request.RoleId,

                IsActive = true,

                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            return "User registered successfully.";
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user =
                await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return null;
            }

            bool isPasswordCorrect =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!isPasswordCorrect)
            {
                return null;
            }

            user.LastLogin = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);

           string token = _jwtTokenGenerator.GenerateToken(user);
            return new LoginResponseDto
            {
                UserId = user.UserId,

                FullName = user.FullName,

                Email = user.Email,

                Role = user.Role!.RoleName,

                Token = token
            };
        }
    }
}