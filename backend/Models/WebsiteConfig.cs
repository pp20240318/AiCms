using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models
{
    /// <summary>
    /// 网站配置模型
    /// </summary>
    public class WebsiteConfig : BaseEntity
    {
        /// <summary>
        /// 配置键
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 配置值
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// 配置分组
        /// </summary>
        [MaxLength(50)]
        public string Group { get; set; } = "general";

        /// <summary>
        /// 数据类型
        /// </summary>
        [MaxLength(20)]
        public string DataType { get; set; } = "string";

        /// <summary>
        /// 是否公开（前台可访问）
        /// </summary>
        public bool IsPublic { get; set; } = false;

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int SortOrder { get; set; } = 0;
    }

    /// <summary>
    /// 网站配置常量
    /// </summary>
    public static class WebsiteConfigKeys
    {
        // 基本信息
        public const string SiteName = "site_name";
        public const string SiteDescription = "site_description";
        public const string SiteLogo = "site_logo";
        public const string SiteIcon = "site_icon";
        public const string SiteKeywords = "site_keywords";

        // 联系信息
        public const string ContactEmail = "contact_email";
        public const string ContactPhone = "contact_phone";
        public const string ContactAddress = "contact_address";
        public const string WorkingHours = "working_hours";

        // 社交媒体
        public const string FacebookUrl = "facebook_url";
        public const string TwitterUrl = "twitter_url";
        public const string LinkedinUrl = "linkedin_url";
        public const string WechatQr = "wechat_qr";

        // SEO设置
        public const string DefaultSeoTitle = "default_seo_title";
        public const string DefaultSeoDescription = "default_seo_description";
        public const string GoogleAnalytics = "google_analytics";
        public const string BaiduAnalytics = "baidu_analytics";

        // 功能开关
        public const string EnableComments = "enable_comments";
        public const string EnableContact = "enable_contact";
        public const string EnableNewsletter = "enable_newsletter";
    }
}