using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.Models;
using MyCms.Api.DTOs;
using BCrypt.Net;

namespace MyCms.Api.Services;

public class UserService : IUserService
{
    private readonly CmsDbContext _context;
    
    public UserService(CmsDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .Where(u => (u.Username == username || u.Email == username) && !u.IsDeleted && u.IsActive)
            .FirstOrDefaultAsync();
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;
        
        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id && !u.IsDeleted)
            .FirstOrDefaultAsync();
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Where(u => u.Username == username && !u.IsDeleted)
            .FirstOrDefaultAsync();
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Where(u => u.Email == email && !u.IsDeleted)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Where(ur => !ur.Role.IsDeleted)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }
    
    public async Task<User> CreateAsync(User user, string password)
    {
        // Check if username or email already exists
        var existingUser = await _context.Users
            .Where(u => (u.Username == user.Username || u.Email == user.Email) && !u.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (existingUser != null)
            throw new ArgumentException("Username or email already exists");
        
        // Hash password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<User> UpdateAsync(User user)
    {
        var existingUser = await _context.Users
            .Where(u => u.Id == user.Id && !u.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (existingUser == null)
            throw new ArgumentException("User not found");
        
        // Check if username or email conflicts with other users
        var conflictUser = await _context.Users
            .Where(u => u.Id != user.Id && (u.Username == user.Username || u.Email == user.Email) && !u.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (conflictUser != null)
            throw new ArgumentException("Username or email already exists");
        
        // Update properties
        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.RealName = user.RealName;
        existingUser.Phone = user.Phone;
        existingUser.IsActive = user.IsActive;
        
        await _context.SaveChangesAsync();
        return existingUser;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id && !u.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (user == null)
            return false;
        
        user.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId && !u.IsDeleted)
            .FirstOrDefaultAsync();
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;
        
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(UserListRequest request)
    {
        var query = _context.Users
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        // 搜索过滤
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.Username.Contains(request.Search) ||
                                   u.Email.Contains(request.Search) ||
                                   u.RealName!.Contains(request.Search));
        }

        // 状态过滤
        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive);
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await GetUserRolesAsync(user.Id);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RealName = user.RealName,
                Phone = user.Phone,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}