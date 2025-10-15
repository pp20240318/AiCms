using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;
using System.Security.Claims;

namespace MyCms.Api.Areas.Admin.Controllers;

[Area("Admin")]
[Route("api/admin/[controller]")]
[ApiController]
[Authorize]

public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UploadResponse>>> UploadFile(IFormFile file)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var result = await _fileService.UploadFileAsync(file, userId);
            return Ok(ApiResponse<UploadResponse>.SuccessResult(result, "文件上传成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UploadResponse>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("upload-multiple")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<UploadResponse>>>> UploadMultipleFiles(List<IFormFile> files)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var results = new List<UploadResponse>();
            foreach (var file in files)
            {
                try
                {
                    var result = await _fileService.UploadFileAsync(file, userId);
                    results.Add(result);
                }
                catch (Exception)
                {
                    // Log error but continue with other files
                    results.Add(new UploadResponse 
                    { 
                        FileName = file.FileName,
                        FileInfo = new UploadedFileDto { OriginalName = file.FileName }
                    });
                }
            }

            return Ok(ApiResponse<List<UploadResponse>>.SuccessResult(results, "文件上传完成"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<UploadResponse>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedResult<UploadedFileDto>>>> GetFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? contentType = null,
        [FromQuery] int? userId = null)
    {
        try
        {
            var request = new FileListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                ContentType = contentType,
                UserId = userId
            };

            var result = await _fileService.GetFilesAsync(request);
            return Ok(ApiResponse<PagedResult<UploadedFileDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<UploadedFileDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("recent")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<UploadedFileDto>>>> GetRecentFiles([FromQuery] int count = 10)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ApiResponse<List<UploadedFileDto>>.ErrorResult("无效的用户信息"));
            }

            var files = await _fileService.GetRecentFilesAsync(userId, count);
            return Ok(ApiResponse<List<UploadedFileDto>>.SuccessResult(files));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<UploadedFileDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UploadedFileDto>>> GetFile(int id)
    {
        try
        {
            var file = await _fileService.GetFileByIdAsync(id);
            if (file == null)
            {
                return NotFound(ApiResponse<UploadedFileDto>.ErrorResult("文件不存在"));
            }

            return Ok(ApiResponse<UploadedFileDto>.SuccessResult(file));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UploadedFileDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFile(int id)
    {
        try
        {
            var success = await _fileService.DeleteFileAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("文件不存在"));
            }

            return Ok(ApiResponse.SuccessResult("文件删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}