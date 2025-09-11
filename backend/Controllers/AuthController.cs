using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    
    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid username or password"));
            }
            
            var roles = await _userService.GetUserRolesAsync(user.Id);
            var token = _jwtService.GenerateToken(user, roles);
            
            var response = new LoginResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    RealName = user.RealName,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt
                },
                Roles = roles
            };
            
            return Ok(ApiResponse<LoginResponse>.SuccessResult(response, "Login successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult(ex.Message));
        }
    }
}