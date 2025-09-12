using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannersController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannersController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BannerDto>>>> GetBanners(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var request = new BannerListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                IsActive = isActive
            };

            var result = await _bannerService.GetBannersAsync(request);
            return Ok(ApiResponse<PagedResult<BannerDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<BannerDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<List<BannerDto>>>> GetActiveBanners()
    {
        try
        {
            var banners = await _bannerService.GetActiveBannersAsync();
            return Ok(ApiResponse<List<BannerDto>>.SuccessResult(banners));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<BannerDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BannerDto>>> GetBanner(int id)
    {
        try
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);
            if (banner == null)
            {
                return NotFound(ApiResponse<BannerDto>.ErrorResult("横幅不存在"));
            }

            return Ok(ApiResponse<BannerDto>.SuccessResult(banner));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BannerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BannerDto>>> CreateBanner([FromBody] CreateBannerRequest request)
    {
        try
        {
            var banner = await _bannerService.CreateBannerAsync(request);
            return CreatedAtAction(nameof(GetBanner), new { id = banner.Id },
                ApiResponse<BannerDto>.SuccessResult(banner, "横幅创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BannerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<BannerDto>>> UpdateBanner(int id, [FromBody] CreateBannerRequest request)
    {
        try
        {
            var updateRequest = new UpdateBannerRequest
            {
                Id = id,
                Title = request.Title,
                ImageUrl = request.ImageUrl,
                LinkUrl = request.LinkUrl,
                LinkTarget = request.LinkTarget,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            var banner = await _bannerService.UpdateBannerAsync(updateRequest);
            if (banner == null)
            {
                return NotFound(ApiResponse<BannerDto>.ErrorResult("横幅不存在"));
            }

            return Ok(ApiResponse<BannerDto>.SuccessResult(banner, "横幅更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<BannerDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBanner(int id)
    {
        try
        {
            var success = await _bannerService.DeleteBannerAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("横幅不存在"));
            }

            return Ok(ApiResponse.SuccessResult("横幅删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPatch("{id}/toggle-active")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ToggleActive(int id)
    {
        try
        {
            var success = await _bannerService.ToggleActiveAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("横幅不存在"));
            }

            return Ok(ApiResponse.SuccessResult("横幅状态切换成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}