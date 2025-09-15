using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models
{
    /// <summary>
    /// SEO设置模型
    /// </summary>
    public class SeoSetting : BaseEntity
    {
        /// <summary>
        /// 页面路径/标识
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PagePath { get; set; } = string.Empty;

        /// <summary>
        /// 页面标题
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 页面描述
        /// </summary>
        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [MaxLength(200)]
        public string? Keywords { get; set; }

        /// <summary>
        /// OG标题
        /// </summary>
        [MaxLength(100)]
        public string? OgTitle { get; set; }

        /// <summary>
        /// OG描述
        /// </summary>
        [MaxLength(300)]
        public string? OgDescription { get; set; }

        /// <summary>
        /// OG图片URL
        /// </summary>
        [MaxLength(500)]
        public string? OgImage { get; set; }

        /// <summary>
        /// 结构化数据(JSON-LD)
        /// </summary>
        public string? StructuredData { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }
}