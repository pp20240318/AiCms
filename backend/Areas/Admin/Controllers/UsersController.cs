using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取所有用户（管理员）
        /// </summary>
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

        /// <summary>
        /// 根据ID获取用户（管理员）
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("用户不存在"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// 创建用户（管理员）
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var user = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id },
                    ApiResponse<UserDto>.SuccessResult(user, "用户创建成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// 更新用户（管理员）
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                request.Id = id;
                var user = await _userService.UpdateUserAsync(id, request);
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResult("用户不存在"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(user, "用户更新成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// 删除用户（管理员）
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("用户不存在"));
                }

                return Ok(ApiResponse<object>.SuccessResult(null, "用户删除成功"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

    }
}