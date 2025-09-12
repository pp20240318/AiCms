using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request);
    Task<RoleDto?> UpdateRoleAsync(UpdateRoleRequest request);
    Task<bool> DeleteRoleAsync(int id);
    Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
}