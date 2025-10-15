using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("api/public/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PagesController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public PagesController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取已发布的页面列表（公开接口）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicPageDto>>> GetPublishedPages()
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
        [HttpGet("{slug}")]
        public async Task<ActionResult<PublicPageDto>> GetPublishedPageBySlug(string slug)
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
        /// 根据ID获取已发布的页面（公开接口）
        /// </summary>
        [HttpGet("id/{id}")]
        public async Task<ActionResult<PublicPageDto>> GetPublishedPageById(int id)
        {
            var page = await _context.Pages
                .Where(p => p.Id == id && p.Status == PageStatus.Published)
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
                return NotFound($"Page with id '{id}' not found");
            }

            // 增加访问计数
            var pageEntity = await _context.Pages.FindAsync(id);
            if (pageEntity != null)
            {
                pageEntity.ViewCount++;
                await _context.SaveChangesAsync();
                page.ViewCount = pageEntity.ViewCount;
            }

            return Ok(page);
        }
    }
}