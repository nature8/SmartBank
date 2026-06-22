using SmartBank.Authentication.DTOs;

namespace SmartBank.Authentication.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto request);

        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}