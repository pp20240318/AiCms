using MyCms.Api.Models;

namespace MyCms.Api.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    Task<User> CreateAsync(User user, string password);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}