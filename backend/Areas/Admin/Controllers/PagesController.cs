using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Security.Claims;

namespace MyCms.Api.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize]
    public class PagesController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public PagesController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有页面（管理员）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetPages([FromQuery] PageQueryDto query)
        {
            var queryable = _context.Pages.AsQueryable();

            // 搜索
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                queryable = queryable.Where(p => p.Title.Contains(query.SearchTerm) || p.Content.Contains(query.SearchTerm));
            }

            // 状态过滤
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(p => p.Status == query.Status.Value);
            }

            // 总数
            var totalCount = await queryable.CountAsync();

            // 分页
            var pages = await queryable
                .OrderByDescending(p => p.UpdatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    Status = p.Status,
                    FeaturedImage = p.FeaturedImage,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    SortOrder = p.SortOrder,
                    ViewCount = p.ViewCount,
                    AuthorId = p.CreatedById,
                    AuthorName = "System",
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    PublishedAt = p.PublishedAt
                })
                .ToListAsync();

            return Ok(new
            {
                items = pages,
                totalCount,
                page = query.Page,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            });
        }

        /// <summary>
        /// 根据ID获取页面（管理员）
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PageDto>> GetPage(int id)
        {
            var page = await _context.Pages
                .Where(p => p.Id == id)
                .Select(p => new PageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    Status = p.Status,
                    FeaturedImage = p.FeaturedImage,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    SortOrder = p.SortOrder,
                    ViewCount = p.ViewCount,
                    AuthorId = p.CreatedById,
                    AuthorName = "System",
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    PublishedAt = p.PublishedAt
                })
                .FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound("页面不存在");
            }

            return Ok(page);
        }

        /// <summary>
        /// 创建页面（管理员）
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PageDto>> CreatePage(CreatePageDto createDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int authorId))
            {
                return Unauthorized("无效的用户信息");
            }

            // 检查Slug是否唯一
            if (await _context.Pages.AnyAsync(p => p.Slug == createDto.Slug))
            {
                return BadRequest("页面别名已存在");
            }

            var page = new Page
            {
                Title = createDto.Title,
                Slug = createDto.Slug,
                Content = createDto.Content,
                Excerpt = createDto.Excerpt,
                Status = createDto.Status,
                FeaturedImage = createDto.FeaturedImage,
                SeoTitle = createDto.SeoTitle,
                SeoDescription = createDto.SeoDescription,
                SeoKeywords = createDto.SeoKeywords,
                SortOrder = createDto.SortOrder,
                CreatedById = authorId,
                PublishedAt = createDto.Status == PageStatus.Published ? DateTime.UtcNow : null
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            var pageDto = new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Content = page.Content,
                Excerpt = page.Excerpt,
                Status = page.Status,
                FeaturedImage = page.FeaturedImage,
                SeoTitle = page.SeoTitle,
                SeoDescription = page.SeoDescription,
                SeoKeywords = page.SeoKeywords,
                SortOrder = page.SortOrder,
                ViewCount = page.ViewCount,
                AuthorId = page.CreatedById,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
                PublishedAt = page.PublishedAt
            };

            return CreatedAtAction(nameof(GetPage), new { id = page.Id }, pageDto);
        }

        /// <summary>
        /// 更新页面（管理员）
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PageDto>> UpdatePage(int id, UpdatePageDto updateDto)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound("页面不存在");
            }

            // 检查Slug是否唯一（排除当前页面）
            if (updateDto.Slug != page.Slug && await _context.Pages.AnyAsync(p => p.Slug == updateDto.Slug))
            {
                return BadRequest("页面别名已存在");
            }

            var wasPublished = page.Status == PageStatus.Published;
            var willBePublished = updateDto.Status == PageStatus.Published;

            page.Title = updateDto.Title;
            page.Slug = updateDto.Slug;
            page.Content = updateDto.Content;
            page.Excerpt = updateDto.Excerpt;
            page.Status = updateDto.Status;
            page.FeaturedImage = updateDto.FeaturedImage;
            page.SeoTitle = updateDto.SeoTitle;
            page.SeoDescription = updateDto.SeoDescription;
            page.SeoKeywords = updateDto.SeoKeywords;
            page.SortOrder = updateDto.SortOrder;
            page.UpdatedAt = DateTime.UtcNow;

            // 如果从非发布状态变为发布状态，设置发布时间
            if (!wasPublished && willBePublished)
            {
                page.PublishedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var pageDto = new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Content = page.Content,
                Excerpt = page.Excerpt,
                Status = page.Status,
                FeaturedImage = page.FeaturedImage,
                SeoTitle = page.SeoTitle,
                SeoDescription = page.SeoDescription,
                SeoKeywords = page.SeoKeywords,
                SortOrder = page.SortOrder,
                ViewCount = page.ViewCount,
                AuthorId = page.CreatedById,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt,
                PublishedAt = page.PublishedAt
            };

            return Ok(pageDto);
        }

        /// <summary>
        /// 删除页面（管理员）
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound("页面不存在");
            }

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();

            return Ok("页面删除成功");
        }
    }
}