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
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// 获取所有文章（管理员）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleDto>>>> GetArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] int? categoryId = null)
    {
        try
        {
            var request = new ArticleListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status, // 管理员可以看到所有状态的文章
                CategoryId = categoryId
            };

            var result = await _articleService.GetArticlesAsync(request);
            return Ok(ApiResponse<PagedResult<ArticleDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<ArticleDto>>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// 根据ID获取文章（管理员）
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetArticle(int id)
    {
        try
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound(ApiResponse<ArticleDto>.ErrorResult("文章不存在"));
            }

            return Ok(ApiResponse<ArticleDto>.SuccessResult(article));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleDto>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// 创建文章（管理员）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> CreateArticle([FromBody] CreateArticleRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int authorId))
            {
                return Unauthorized(ApiResponse<ArticleDto>.ErrorResult("无效的用户信息"));
            }

            var article = await _articleService.CreateArticleAsync(request, authorId);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id },
                ApiResponse<ArticleDto>.SuccessResult(article, "文章创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleDto>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// 更新文章（管理员）
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> UpdateArticle(int id, [FromBody] CreateArticleRequest request)
    {
        try
        {
            var updateRequest = new UpdateArticleRequest
            {
                Id = id,
                Title = request.Title,
                Summary = request.Summary,
                Content = request.Content,
                CategoryId = request.CategoryId,
                IsPublished = request.IsPublished,
                CoverImage = request.CoverImage,
                SeoTitle = request.SeoTitle,
                SeoDescription = request.SeoDescription,
                SeoKeywords = request.SeoKeywords,
                SortOrder = request.SortOrder
            };

            var article = await _articleService.UpdateArticleAsync(updateRequest);
            if (article == null)
            {
                return NotFound(ApiResponse<ArticleDto>.ErrorResult("文章不存在"));
            }

            return Ok(ApiResponse<ArticleDto>.SuccessResult(article, "文章更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleDto>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// 删除文章（管理员）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteArticle(int id)
    {
        try
        {
            var success = await _articleService.DeleteArticleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("文章不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "文章删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

}