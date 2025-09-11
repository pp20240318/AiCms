using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;
using System.Security.Claims;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

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
                Status = status,
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

    [HttpPost]
    [Authorize]
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

    [HttpPut("{id}")]
    [Authorize]
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
                CoverImage = request.CoverImage,
                CategoryId = request.CategoryId,
                IsPublished = request.IsPublished,
                PublishedAt = request.PublishedAt,
                SortOrder = request.SortOrder,
                SeoTitle = request.SeoTitle,
                SeoDescription = request.SeoDescription,
                SeoKeywords = request.SeoKeywords
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

    [HttpDelete("{id}")]
    [Authorize]
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

    [HttpPatch("{id}/publish")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> PublishArticle(int id)
    {
        try
        {
            var success = await _articleService.PublishArticleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("文章不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "文章发布成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPatch("{id}/unpublish")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> UnpublishArticle(int id)
    {
        try
        {
            var success = await _articleService.UnpublishArticleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("文章不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "文章取消发布成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}