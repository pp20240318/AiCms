using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Controllers
{
    [ApiController]
    [Area("Admin")]
[Route("api/admin/[controller]")]
[Authorize]
    public class WebsiteConfigController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public WebsiteConfigController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取公开配置（前台使用）
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PublicConfigDto>>> GetPublicConfigs()
        {
            var configs = await _context.WebsiteConfigs
                .Where(c => c.IsPublic)
                .OrderBy(c => c.Group)
                .ThenBy(c => c.SortOrder)
                .Select(c => new PublicConfigDto
                {
                    Key = c.Key,
                    Value = c.Value,
                    DataType = c.DataType
                })
                .ToListAsync();

            return Ok(configs);
        }

        /// <summary>
        /// 根据Key获取公开配置
        /// </summary>
        [HttpGet("public/{key}")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicConfigDto>> GetPublicConfigByKey(string key)
        {
            var config = await _context.WebsiteConfigs
                .Where(c => c.Key == key && c.IsPublic)
                .Select(c => new PublicConfigDto
                {
                    Key = c.Key,
                    Value = c.Value,
                    DataType = c.DataType
                })
                .FirstOrDefaultAsync();

            if (config == null)
            {
                return NotFound($"Public config with key '{key}' not found");
            }

            return Ok(config);
        }

        /// <summary>
        /// 获取所有配置（管理员）
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetConfigs([FromQuery] WebsiteConfigQueryDto query)
        {
            var queryable = _context.WebsiteConfigs.AsQueryable();

            // 分组筛选
            if (!string.IsNullOrWhiteSpace(query.Group))
            {
                queryable = queryable.Where(c => c.Group == query.Group);
            }

            // 公开状态筛选
            if (query.IsPublic.HasValue)
            {
                queryable = queryable.Where(c => c.IsPublic == query.IsPublic.Value);
            }

            // 关键词搜索
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                queryable = queryable.Where(c =>
                    c.Key.Contains(query.SearchTerm) ||
                    c.Description!.Contains(query.SearchTerm) ||
                    c.Value!.Contains(query.SearchTerm));
            }

            var totalCount = await queryable.CountAsync();

            var configs = await queryable
                .OrderBy(c => c.Group)
                .ThenBy(c => c.SortOrder)
                .ThenBy(c => c.Key)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new WebsiteConfigDto
                {
                    Id = c.Id,
                    Key = c.Key,
                    Value = c.Value,
                    Description = c.Description,
                    Group = c.Group,
                    DataType = c.DataType,
                    IsPublic = c.IsPublic,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                data = configs,
                totalCount,
                pageCount = (int)Math.Ceiling((double)totalCount / query.PageSize),
                currentPage = query.Page,
                pageSize = query.PageSize
            });
        }

        /// <summary>
        /// 获取配置分组列表
        /// </summary>
        [HttpGet("groups")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<string>>> GetConfigGroups()
        {
            var groups = await _context.WebsiteConfigs
                .Select(c => c.Group)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            return Ok(groups);
        }

        /// <summary>
        /// 获取指定配置
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<WebsiteConfigDto>> GetConfig(int id)
        {
            var config = await _context.WebsiteConfigs
                .Where(c => c.Id == id)
                .Select(c => new WebsiteConfigDto
                {
                    Id = c.Id,
                    Key = c.Key,
                    Value = c.Value,
                    Description = c.Description,
                    Group = c.Group,
                    DataType = c.DataType,
                    IsPublic = c.IsPublic,
                    SortOrder = c.SortOrder,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (config == null)
            {
                return NotFound();
            }

            return Ok(config);
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WebsiteConfigDto>> CreateConfig(CreateWebsiteConfigDto createDto)
        {
            // 检查Key是否已存在
            var existingConfig = await _context.WebsiteConfigs
                .AnyAsync(c => c.Key == createDto.Key);

            if (existingConfig)
            {
                return BadRequest($"Config with key '{createDto.Key}' already exists");
            }

            var config = new WebsiteConfig
            {
                Key = createDto.Key,
                Value = createDto.Value,
                Description = createDto.Description,
                Group = createDto.Group,
                DataType = createDto.DataType,
                IsPublic = createDto.IsPublic,
                SortOrder = createDto.SortOrder
            };

            _context.WebsiteConfigs.Add(config);
            await _context.SaveChangesAsync();

            var configDto = new WebsiteConfigDto
            {
                Id = config.Id,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description,
                Group = config.Group,
                DataType = config.DataType,
                IsPublic = config.IsPublic,
                SortOrder = config.SortOrder,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt
            };

            return CreatedAtAction(nameof(GetConfig), new { id = config.Id }, configDto);
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateConfig(int id, UpdateWebsiteConfigDto updateDto)
        {
            var config = await _context.WebsiteConfigs.FindAsync(id);
            if (config == null)
            {
                return NotFound();
            }

            config.Value = updateDto.Value;
            config.Description = updateDto.Description;
            config.Group = updateDto.Group;
            config.DataType = updateDto.DataType;
            config.IsPublic = updateDto.IsPublic;
            config.SortOrder = updateDto.SortOrder;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ConfigExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// 批量更新配置
        /// </summary>
        [HttpPut("batch")]
        [Authorize]
        public async Task<IActionResult> UpdateConfigsBatch([FromBody] Dictionary<string, string> configs)
        {
            foreach (var kvp in configs)
            {
                var config = await _context.WebsiteConfigs
                    .FirstOrDefaultAsync(c => c.Key == kvp.Key);

                if (config != null)
                {
                    config.Value = kvp.Value;
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// 删除配置
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteConfig(int id)
        {
            var config = await _context.WebsiteConfigs.FindAsync(id);
            if (config == null)
            {
                return NotFound();
            }

            _context.WebsiteConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 初始化默认配置
        /// </summary>
        [HttpPost("initialize")]
        [Authorize]
        public async Task<IActionResult> InitializeDefaultConfigs()
        {
            var defaultConfigs = new[]
            {
                new WebsiteConfig { Key = WebsiteConfigKeys.SiteName, Value = "My CMS Website", Description = "网站名称", Group = "basic", DataType = "string", IsPublic = true, SortOrder = 1 },
                new WebsiteConfig { Key = WebsiteConfigKeys.SiteDescription, Value = "A modern CMS website", Description = "网站描述", Group = "basic", DataType = "string", IsPublic = true, SortOrder = 2 },
                new WebsiteConfig { Key = WebsiteConfigKeys.SiteKeywords, Value = "cms,website,management", Description = "网站关键词", Group = "seo", DataType = "string", IsPublic = true, SortOrder = 1 },
                new WebsiteConfig { Key = WebsiteConfigKeys.ContactEmail, Value = "contact@example.com", Description = "联系邮箱", Group = "contact", DataType = "email", IsPublic = true, SortOrder = 1 },
                new WebsiteConfig { Key = WebsiteConfigKeys.ContactPhone, Value = "", Description = "联系电话", Group = "contact", DataType = "string", IsPublic = true, SortOrder = 2 },
                new WebsiteConfig { Key = WebsiteConfigKeys.ContactAddress, Value = "", Description = "联系地址", Group = "contact", DataType = "string", IsPublic = true, SortOrder = 3 },
                new WebsiteConfig { Key = WebsiteConfigKeys.EnableContact, Value = "true", Description = "启用联系我们功能", Group = "features", DataType = "boolean", IsPublic = false, SortOrder = 1 },
                new WebsiteConfig { Key = WebsiteConfigKeys.EnableComments, Value = "false", Description = "启用评论功能", Group = "features", DataType = "boolean", IsPublic = false, SortOrder = 2 }
            };

            foreach (var config in defaultConfigs)
            {
                var existing = await _context.WebsiteConfigs
                    .AnyAsync(c => c.Key == config.Key);

                if (!existing)
                {
                    _context.WebsiteConfigs.Add(config);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Default configurations initialized successfully");
        }

        private async Task<bool> ConfigExists(int id)
        {
            return await _context.WebsiteConfigs.AnyAsync(e => e.Id == id);
        }
    }
}