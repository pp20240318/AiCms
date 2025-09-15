using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SeoSettingsController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public SeoSettingsController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有SEO设置
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeoSettingDto>>> GetSeoSettings()
        {
            var seoSettings = await _context.SeoSettings
                .OrderBy(s => s.PagePath)
                .Select(s => new SeoSettingDto
                {
                    Id = s.Id,
                    PagePath = s.PagePath,
                    Title = s.Title,
                    Description = s.Description,
                    Keywords = s.Keywords,
                    OgTitle = s.OgTitle,
                    OgDescription = s.OgDescription,
                    OgImage = s.OgImage,
                    StructuredData = s.StructuredData,
                    IsEnabled = s.IsEnabled,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return Ok(seoSettings);
        }

        /// <summary>
        /// 根据页面路径获取SEO设置
        /// </summary>
        [HttpGet("by-path/{*pagePath}")]
        [AllowAnonymous]
        public async Task<ActionResult<SeoSettingDto>> GetSeoSettingByPath(string pagePath)
        {
            var seoSetting = await _context.SeoSettings
                .Where(s => s.PagePath == pagePath && s.IsEnabled)
                .Select(s => new SeoSettingDto
                {
                    Id = s.Id,
                    PagePath = s.PagePath,
                    Title = s.Title,
                    Description = s.Description,
                    Keywords = s.Keywords,
                    OgTitle = s.OgTitle,
                    OgDescription = s.OgDescription,
                    OgImage = s.OgImage,
                    StructuredData = s.StructuredData,
                    IsEnabled = s.IsEnabled,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (seoSetting == null)
            {
                return NotFound($"SEO setting for path '{pagePath}' not found");
            }

            return Ok(seoSetting);
        }

        /// <summary>
        /// 获取指定ID的SEO设置
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SeoSettingDto>> GetSeoSetting(int id)
        {
            var seoSetting = await _context.SeoSettings
                .Where(s => s.Id == id)
                .Select(s => new SeoSettingDto
                {
                    Id = s.Id,
                    PagePath = s.PagePath,
                    Title = s.Title,
                    Description = s.Description,
                    Keywords = s.Keywords,
                    OgTitle = s.OgTitle,
                    OgDescription = s.OgDescription,
                    OgImage = s.OgImage,
                    StructuredData = s.StructuredData,
                    IsEnabled = s.IsEnabled,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (seoSetting == null)
            {
                return NotFound();
            }

            return Ok(seoSetting);
        }

        /// <summary>
        /// 创建SEO设置
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SeoSettingDto>> CreateSeoSetting(CreateSeoSettingDto createDto)
        {
            // 检查页面路径是否已存在
            var existingSetting = await _context.SeoSettings
                .AnyAsync(s => s.PagePath == createDto.PagePath);

            if (existingSetting)
            {
                return BadRequest($"SEO setting for path '{createDto.PagePath}' already exists");
            }

            var seoSetting = new SeoSetting
            {
                PagePath = createDto.PagePath,
                Title = createDto.Title,
                Description = createDto.Description,
                Keywords = createDto.Keywords,
                OgTitle = createDto.OgTitle,
                OgDescription = createDto.OgDescription,
                OgImage = createDto.OgImage,
                StructuredData = createDto.StructuredData,
                IsEnabled = createDto.IsEnabled
            };

            _context.SeoSettings.Add(seoSetting);
            await _context.SaveChangesAsync();

            var seoSettingDto = new SeoSettingDto
            {
                Id = seoSetting.Id,
                PagePath = seoSetting.PagePath,
                Title = seoSetting.Title,
                Description = seoSetting.Description,
                Keywords = seoSetting.Keywords,
                OgTitle = seoSetting.OgTitle,
                OgDescription = seoSetting.OgDescription,
                OgImage = seoSetting.OgImage,
                StructuredData = seoSetting.StructuredData,
                IsEnabled = seoSetting.IsEnabled,
                CreatedAt = seoSetting.CreatedAt,
                UpdatedAt = seoSetting.UpdatedAt
            };

            return CreatedAtAction(nameof(GetSeoSetting), new { id = seoSetting.Id }, seoSettingDto);
        }

        /// <summary>
        /// 更新SEO设置
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSeoSetting(int id, UpdateSeoSettingDto updateDto)
        {
            var seoSetting = await _context.SeoSettings.FindAsync(id);
            if (seoSetting == null)
            {
                return NotFound();
            }

            seoSetting.Title = updateDto.Title;
            seoSetting.Description = updateDto.Description;
            seoSetting.Keywords = updateDto.Keywords;
            seoSetting.OgTitle = updateDto.OgTitle;
            seoSetting.OgDescription = updateDto.OgDescription;
            seoSetting.OgImage = updateDto.OgImage;
            seoSetting.StructuredData = updateDto.StructuredData;
            seoSetting.IsEnabled = updateDto.IsEnabled;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SeoSettingExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// 删除SEO设置
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeoSetting(int id)
        {
            var seoSetting = await _context.SeoSettings.FindAsync(id);
            if (seoSetting == null)
            {
                return NotFound();
            }

            _context.SeoSettings.Remove(seoSetting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> SeoSettingExists(int id)
        {
            return await _context.SeoSettings.AnyAsync(e => e.Id == id);
        }
    }
}