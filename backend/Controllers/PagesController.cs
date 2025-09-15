using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Security.Claims;

namespace MyCms.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public PagesController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取已发布的页面（公开接口）
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicPageDto>>> GetPublicPages()
        {
            var pages = await _context.Pages
                .Where(p => p.Status == PageStatus.Published)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.PublishedAt)
                .Select(p => new PublicPageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    FeaturedImage = p.FeaturedImage,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount
                })
                .ToListAsync();

            return Ok(pages);
        }

        /// <summary>
        /// 根据Slug获取已发布的页面（公开接口）
        /// </summary>
        [HttpGet("public/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicPageDto>> GetPublicPageBySlug(string slug)
        {
            var page = await _context.Pages
                .Where(p => p.Slug == slug && p.Status == PageStatus.Published)
                .Select(p => new PublicPageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    FeaturedImage = p.FeaturedImage,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount
                })
                .FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound($"Page with slug '{slug}' not found");
            }

            // 增加访问计数
            var pageEntity = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);
            if (pageEntity != null)
            {
                pageEntity.ViewCount++;
                await _context.SaveChangesAsync();
                page.ViewCount = pageEntity.ViewCount;
            }

            return Ok(page);
        }

        /// <summary>
        /// 获取所有页面（管理员）
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetPages([FromQuery] PageQueryDto query)
        {
            var queryable = _context.Pages.AsQueryable();

            // 状态筛选
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(p => p.Status == query.Status.Value);
            }

            // 模板筛选
            if (!string.IsNullOrWhiteSpace(query.Template))
            {
                queryable = queryable.Where(p => p.Template == query.Template);
            }

            // 菜单显示筛选
            if (query.ShowInMenu.HasValue)
            {
                queryable = queryable.Where(p => p.ShowInMenu == query.ShowInMenu.Value);
            }

            // 父页面筛选
            if (query.ParentId.HasValue)
            {
                queryable = queryable.Where(p => p.ParentId == query.ParentId.Value);
            }

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                queryable = queryable.Where(p =>
                    p.Title.Contains(query.SearchTerm) ||
                    p.Content.Contains(query.SearchTerm) ||
                    p.Excerpt!.Contains(query.SearchTerm));
            }

            var totalCount = await queryable.CountAsync();

            var pages = await queryable
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    FeaturedImage = p.FeaturedImage,
                    Template = p.Template,
                    Status = p.Status,
                    SortOrder = p.SortOrder,
                    ShowInMenu = p.ShowInMenu,
                    ParentId = p.ParentId,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    PublishedAt = p.PublishedAt,
                    CreatedById = p.CreatedById,
                    UpdatedById = p.UpdatedById,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                data = pages,
                totalCount,
                pageCount = (int)Math.Ceiling((double)totalCount / query.PageSize),
                currentPage = query.Page,
                pageSize = query.PageSize
            });
        }

        /// <summary>
        /// 获取指定页面
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
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
                    FeaturedImage = p.FeaturedImage,
                    Template = p.Template,
                    Status = p.Status,
                    SortOrder = p.SortOrder,
                    ShowInMenu = p.ShowInMenu,
                    ParentId = p.ParentId,
                    SeoTitle = p.SeoTitle,
                    SeoDescription = p.SeoDescription,
                    SeoKeywords = p.SeoKeywords,
                    PublishedAt = p.PublishedAt,
                    CreatedById = p.CreatedById,
                    UpdatedById = p.UpdatedById,
                    ViewCount = p.ViewCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound();
            }

            return Ok(page);
        }

        /// <summary>
        /// 创建页面
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PageDto>> CreatePage(CreatePageDto createDto)
        {
            // 检查Slug是否已存在
            var existingPage = await _context.Pages
                .AnyAsync(p => p.Slug == createDto.Slug);

            if (existingPage)
            {
                return BadRequest($"Page with slug '{createDto.Slug}' already exists");
            }

            var page = new Page
            {
                Title = createDto.Title,
                Slug = createDto.Slug,
                Content = createDto.Content,
                Excerpt = createDto.Excerpt,
                FeaturedImage = createDto.FeaturedImage,
                Template = createDto.Template,
                Status = createDto.Status,
                SortOrder = createDto.SortOrder,
                ShowInMenu = createDto.ShowInMenu,
                ParentId = createDto.ParentId,
                SeoTitle = createDto.SeoTitle,
                SeoDescription = createDto.SeoDescription,
                SeoKeywords = createDto.SeoKeywords,
                CreatedById = GetCurrentUserId()
            };

            if (createDto.Status == PageStatus.Published)
            {
                page.PublishedAt = DateTime.UtcNow;
            }

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            var pageDto = new PageDto
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Content = page.Content,
                Excerpt = page.Excerpt,
                FeaturedImage = page.FeaturedImage,
                Template = page.Template,
                Status = page.Status,
                SortOrder = page.SortOrder,
                ShowInMenu = page.ShowInMenu,
                ParentId = page.ParentId,
                SeoTitle = page.SeoTitle,
                SeoDescription = page.SeoDescription,
                SeoKeywords = page.SeoKeywords,
                PublishedAt = page.PublishedAt,
                CreatedById = page.CreatedById,
                UpdatedById = page.UpdatedById,
                ViewCount = page.ViewCount,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPage), new { id = page.Id }, pageDto);
        }

        /// <summary>
        /// 更新页面
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePage(int id, UpdatePageDto updateDto)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            // 检查Slug是否与其他页面冲突
            var existingPage = await _context.Pages
                .AnyAsync(p => p.Slug == updateDto.Slug && p.Id != id);

            if (existingPage)
            {
                return BadRequest($"Page with slug '{updateDto.Slug}' already exists");
            }

            var wasPublished = page.Status == PageStatus.Published;

            page.Title = updateDto.Title;
            page.Slug = updateDto.Slug;
            page.Content = updateDto.Content;
            page.Excerpt = updateDto.Excerpt;
            page.FeaturedImage = updateDto.FeaturedImage;
            page.Template = updateDto.Template;
            page.Status = updateDto.Status;
            page.SortOrder = updateDto.SortOrder;
            page.ShowInMenu = updateDto.ShowInMenu;
            page.ParentId = updateDto.ParentId;
            page.SeoTitle = updateDto.SeoTitle;
            page.SeoDescription = updateDto.SeoDescription;
            page.SeoKeywords = updateDto.SeoKeywords;
            page.UpdatedById = GetCurrentUserId();

            // 如果从非发布状态变为发布状态，设置发布时间
            if (!wasPublished && updateDto.Status == PageStatus.Published)
            {
                page.PublishedAt = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PageExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// 删除页面
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> PageExists(int id)
        {
            return await _context.Pages.AnyAsync(e => e.Id == id);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}