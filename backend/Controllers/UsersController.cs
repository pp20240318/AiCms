using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;
using MyCms.Api.Models;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var request = new UserListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                IsActive = isActive
            };

            var result = await _userService.GetUsersAsync(request);
            return Ok(ApiResponse<PagedResult<UserDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<UserDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult("用户不存在"));
            }

            var roles = await _userService.GetUserRolesAsync(user.Id);
            var userDto = new UserDto
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
            };

            return Ok(ApiResponse<UserDto>.SuccessResult(userDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                RealName = request.RealName,
                Phone = request.Phone,
                IsActive = request.IsActive
            };

            var createdUser = await _userService.CreateAsync(user, request.Password);
            
            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                RealName = createdUser.RealName,
                Phone = createdUser.Phone,
                IsActive = createdUser.IsActive,
                LastLoginAt = createdUser.LastLoginAt,
                CreatedAt = createdUser.CreatedAt,
                Roles = request.Roles
            };

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id },
                ApiResponse<UserDto>.SuccessResult(userDto, "用户创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult("ID不匹配"));
            }

            var user = new User
            {
                Id = request.Id,
                Username = request.Username,
                Email = request.Email,
                RealName = request.RealName,
                Phone = request.Phone,
                IsActive = request.IsActive
            };

            var updatedUser = await _userService.UpdateAsync(user);
            var roles = await _userService.GetUserRolesAsync(updatedUser.Id);
            
            var userDto = new UserDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                RealName = updatedUser.RealName,
                Phone = updatedUser.Phone,
                IsActive = updatedUser.IsActive,
                LastLoginAt = updatedUser.LastLoginAt,
                CreatedAt = updatedUser.CreatedAt,
                Roles = roles
            };

            return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "用户更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
    {
        try
        {
            var success = await _userService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("用户不存在"));
            }

            return Ok(ApiResponse.SuccessResult("用户删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("{id}/change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        try
        {
            var success = await _userService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);
            if (!success)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("当前密码错误或用户不存在"));
            }

            return Ok(ApiResponse.SuccessResult("密码修改成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}