using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.DTOs
{
    public class SeoSettingDto
    {
        public int Id { get; set; }
        public string PagePath { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public string? OgImage { get; set; }
        public string? StructuredData { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSeoSettingDto
    {
        [Required]
        [MaxLength(200)]
        public string PagePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Keywords { get; set; }

        [MaxLength(100)]
        public string? OgTitle { get; set; }

        [MaxLength(300)]
        public string? OgDescription { get; set; }

        [MaxLength(500)]
        public string? OgImage { get; set; }

        public string? StructuredData { get; set; }

        public bool IsEnabled { get; set; } = true;
    }

    public class UpdateSeoSettingDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Keywords { get; set; }

        [MaxLength(100)]
        public string? OgTitle { get; set; }

        [MaxLength(300)]
        public string? OgDescription { get; set; }

        [MaxLength(500)]
        public string? OgImage { get; set; }

        public string? StructuredData { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}