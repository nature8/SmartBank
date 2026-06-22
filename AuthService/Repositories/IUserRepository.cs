using SmartBank.Authentication.Models;

namespace SmartBank.Authentication.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByIdAsync(int id);

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task<bool> EmailExistsAsync(string email);

        Task SaveChangesAsync();
    }
}