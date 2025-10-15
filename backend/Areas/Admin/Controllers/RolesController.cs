using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetRoles()
    {
        try
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResult(roles));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<RoleDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(int id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponse<RoleDto>.ErrorResult("角色不存在"));
            }

            return Ok(ApiResponse<RoleDto>.SuccessResult(role));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            var role = await _roleService.CreateRoleAsync(request);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id },
                ApiResponse<RoleDto>.SuccessResult(role, "角色创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(int id, [FromBody] CreateRoleRequest request)
    {
        try
        {
            var updateRequest = new UpdateRoleRequest
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                PermissionIds = request.PermissionIds
            };

            var role = await _roleService.UpdateRoleAsync(updateRequest);
            if (role == null)
            {
                return NotFound(ApiResponse<RoleDto>.ErrorResult("角色不存在"));
            }

            return Ok(ApiResponse<RoleDto>.SuccessResult(role, "角色更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<RoleDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteRole(int id)
    {
        try
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("角色不存在"));
            }

            return Ok(ApiResponse.SuccessResult("角色删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}