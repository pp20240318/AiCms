using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models
{
    /// <summary>
    /// 自定义页面模型
    /// </summary>
    public class Page : BaseEntity
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 页面路径/URL
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// 页面内容
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 页面摘要
        /// </summary>
        [MaxLength(500)]
        public string? Excerpt { get; set; }

        /// <summary>
        /// 特色图片URL
        /// </summary>
        [MaxLength(500)]
        public string? FeaturedImage { get; set; }

        /// <summary>
        /// 页面模板
        /// </summary>
        [MaxLength(50)]
        public string Template { get; set; } = "default";

        /// <summary>
        /// 页面状态
        /// </summary>
        public PageStatus Status { get; set; } = PageStatus.Draft;

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 是否显示在导航菜单
        /// </summary>
        public bool ShowInMenu { get; set; } = false;

        /// <summary>
        /// 父页面ID（用于页面层级）
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// SEO标题
        /// </summary>
        [MaxLength(100)]
        public string? SeoTitle { get; set; }

        /// <summary>
        /// SEO描述
        /// </summary>
        [MaxLength(300)]
        public string? SeoDescription { get; set; }

        /// <summary>
        /// SEO关键词
        /// </summary>
        [MaxLength(200)]
        public string? SeoKeywords { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PublishedAt { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        /// 更新人ID
        /// </summary>
        public int? UpdatedById { get; set; }

        /// <summary>
        /// 访问计数
        /// </summary>
        public int ViewCount { get; set; } = 0;
    }

    /// <summary>
    /// 页面状态枚举
    /// </summary>
    public enum PageStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,

        /// <summary>
        /// 已发布
        /// </summary>
        Published = 1,

        /// <summary>
        /// 已归档
        /// </summary>
        Archived = 2
    }
}