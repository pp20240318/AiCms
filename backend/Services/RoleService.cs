using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public class RoleService : IRoleService
{
    private readonly CmsDbContext _context;

    public RoleService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsSystem = r.IsSystem,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt ?? r.CreatedAt,
            Permissions = r.RolePermissions
                .Where(rp => !rp.Permission.IsDeleted)
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Code = rp.Permission.Code,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    Module = rp.Permission.Module,
                    CreatedAt = rp.Permission.CreatedAt
                })
        });
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(r => r.Id == id && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (role == null) return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystem = role.IsSystem,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt ?? role.CreatedAt,
            Permissions = role.RolePermissions
                .Where(rp => !rp.Permission.IsDeleted)
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Code = rp.Permission.Code,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    Module = rp.Permission.Module,
                    CreatedAt = rp.Permission.CreatedAt
                })
        };
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request)
    {
        // Check if role name already exists
        var existingRole = await _context.Roles
            .Where(r => r.Name == request.Name && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingRole != null)
            throw new ArgumentException("角色名称已存在");

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Add permissions
        if (request.PermissionIds.Any())
        {
            var rolePermissions = request.PermissionIds.Select(permissionId => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });

            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        return await GetRoleByIdAsync(role.Id) ?? throw new Exception("创建角色失败");
    }

    public async Task<RoleDto?> UpdateRoleAsync(UpdateRoleRequest request)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .Where(r => r.Id == request.Id && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (role == null) return null;

        // Check if name conflicts with other roles
        var conflictRole = await _context.Roles
            .Where(r => r.Id != request.Id && r.Name == request.Name && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (conflictRole != null)
            throw new ArgumentException("角色名称已存在");

        // Update role properties
        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;

        // Update permissions
        _context.RolePermissions.RemoveRange(role.RolePermissions);
        
        if (request.PermissionIds.Any())
        {
            var rolePermissions = request.PermissionIds.Select(permissionId => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });

            _context.RolePermissions.AddRange(rolePermissions);
        }

        await _context.SaveChangesAsync();
        return await GetRoleByIdAsync(role.Id);
    }

    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.Roles
            .Where(r => r.Id == id && !r.IsDeleted)
            .FirstOrDefaultAsync();

        if (role == null) return false;

        if (role.IsSystem)
            throw new ArgumentException("系统角色不能删除");

        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
    {
        var permissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Name)
            .ToListAsync();

        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            Module = p.Module,
            CreatedAt = p.CreatedAt
        });
    }
}