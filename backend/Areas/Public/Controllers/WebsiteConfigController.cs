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
    public class WebsiteConfigController : ControllerBase
    {
        private readonly CmsDbContext _context;

        public WebsiteConfigController(CmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取公开的网站配置（公开接口）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<object>> GetPublicConfig()
        {
            var configs = await _context.WebsiteConfigs
                .Where(c => c.IsPublic == true)
                .ToDictionaryAsync(c => c.Key, c => c.Value);

            return Ok(configs);
        }

        /// <summary>
        /// 根据Key获取特定的公开配置（公开接口）
        /// </summary>
        [HttpGet("{key}")]
        public async Task<ActionResult<string>> GetPublicConfigByKey(string key)
        {
            var config = await _context.WebsiteConfigs
                .Where(c => c.Key == key && c.IsPublic == true)
                .FirstOrDefaultAsync();

            if (config == null)
            {
                return NotFound($"配置项 '{key}' 不存在或不可公开访问");
            }

            return Ok(config.Value);
        }
    }
}