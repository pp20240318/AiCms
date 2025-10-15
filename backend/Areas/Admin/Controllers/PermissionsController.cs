using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Route("api/admin/[controller]")]
[Authorize]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IRoleService _roleService;

    public PermissionsController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetPermissions()
    {
        try
        {
            var permissions = await _roleService.GetAllPermissionsAsync();
            return Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResult(permissions));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<PermissionDto>>.ErrorResult(ex.Message));
        }
    }
}