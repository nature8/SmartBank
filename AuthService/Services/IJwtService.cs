using SmartBank.Authentication.Models;

namespace SmartBank.Authentication.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}