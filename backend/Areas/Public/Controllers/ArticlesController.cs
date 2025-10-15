using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Areas.Public.Controllers;

[Area("Public")]
[Route("api/public/[controller]")]
[ApiController]
[AllowAnonymous]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// 获取已发布的文章列表（公开接口）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleDto>>>> GetPublishedArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null)
    {
        try
        {
            var request = new ArticleListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = "published", // 只返回已发布的文章
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
    /// 根据ID获取已发布的文章（公开接口）
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ArticleDto>>> GetPublishedArticle(int id)
    {
        try
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound(ApiResponse<ArticleDto>.ErrorResult("文章不存在"));
            }

            // 检查文章是否已发布
            if (!article.IsPublished)
            {
                return NotFound(ApiResponse<ArticleDto>.ErrorResult("文章不存在"));
            }

            // 增加访问计数
            await _articleService.IncrementViewCountAsync(id);

            return Ok(ApiResponse<ArticleDto>.SuccessResult(article));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleDto>.ErrorResult(ex.Message));
        }
    }

    /// <summary>
    /// 根据分类获取已发布的文章
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<PagedResult<ArticleDto>>>> GetArticlesByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var request = new ArticleListRequest
            {
                Page = page,
                PageSize = pageSize,
                Status = "published",
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
}