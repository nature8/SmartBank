using Microsoft.IdentityModel.Tokens;
using SmartBank.Authentication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartBank.Authentication.Services
{
    public class JwtService : IJwtService
    {

        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                    user.UserId.ToString()),

                new Claim(ClaimTypes.Name,
                    user.FullName),

                new Claim(ClaimTypes.Email,
                    user.Email),

                new Claim(ClaimTypes.Role,
                    user.Role!.RoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],

                audience: _configuration["Jwt:Audience"],

                claims: claims,

                expires: DateTime.UtcNow.AddHours(2),

                signingCredentials:
                    new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}