using System.ComponentModel.DataAnnotations;
using MyCms.Api.Models;

namespace MyCms.Api.DTOs
{
    public class PageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? FeaturedImage { get; set; }
        public string Template { get; set; } = "default";
        public PageStatus Status { get; set; }
        public int SortOrder { get; set; }
        public bool ShowInMenu { get; set; }
        public int? ParentId { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePageDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        [MaxLength(500)]
        public string? FeaturedImage { get; set; }

        [MaxLength(50)]
        public string Template { get; set; } = "default";

        public PageStatus Status { get; set; } = PageStatus.Draft;

        public int SortOrder { get; set; } = 0;

        public bool ShowInMenu { get; set; } = false;

        public int? ParentId { get; set; }

        [MaxLength(100)]
        public string? SeoTitle { get; set; }

        [MaxLength(300)]
        public string? SeoDescription { get; set; }

        [MaxLength(200)]
        public string? SeoKeywords { get; set; }
    }

    public class UpdatePageDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        [MaxLength(500)]
        public string? FeaturedImage { get; set; }

        [MaxLength(50)]
        public string Template { get; set; } = "default";

        public PageStatus Status { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool ShowInMenu { get; set; } = false;

        public int? ParentId { get; set; }

        [MaxLength(100)]
        public string? SeoTitle { get; set; }

        [MaxLength(300)]
        public string? SeoDescription { get; set; }

        [MaxLength(200)]
        public string? SeoKeywords { get; set; }
    }

    public class PageQueryDto
    {
        public PageStatus? Status { get; set; }
        public string? Template { get; set; }
        public bool? ShowInMenu { get; set; }
        public int? ParentId { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PublicPageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? FeaturedImage { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywords { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
    }
}