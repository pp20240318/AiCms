using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models
{
    /// <summary>
    /// 联系我们消息模型
    /// </summary>
    public class Contact : BaseEntity
    {
        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [MaxLength(100)]
        public string? Company { get; set; }

        /// <summary>
        /// 消息主题
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// 消息内容
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 处理状态
        /// </summary>
        public ContactStatus Status { get; set; } = ContactStatus.New;

        /// <summary>
        /// 回复内容
        /// </summary>
        public string? Reply { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? RepliedAt { get; set; }

        /// <summary>
        /// 回复人ID
        /// </summary>
        public int? RepliedById { get; set; }

        /// <summary>
        /// 客户端IP地址
        /// </summary>
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        [MaxLength(500)]
        public string? UserAgent { get; set; }
    }

    /// <summary>
    /// 联系消息状态枚举
    /// </summary>
    public enum ContactStatus
    {
        /// <summary>
        /// 新消息
        /// </summary>
        New = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 1,

        /// <summary>
        /// 已回复
        /// </summary>
        Replied = 2,

        /// <summary>
        /// 已关闭
        /// </summary>
        Closed = 3
    }
}