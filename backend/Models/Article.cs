using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class Article : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Summary { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? CoverImage { get; set; }
    
    public int CategoryId { get; set; }
    
    public int AuthorId { get; set; }
    
    public bool IsPublished { get; set; } = false;
    
    public DateTime? PublishedAt { get; set; }
    
    public int ViewCount { get; set; } = 0;
    
    public int SortOrder { get; set; } = 0;
    
    [StringLength(200)]
    public string? SeoTitle { get; set; }
    
    [StringLength(500)]
    public string? SeoDescription { get; set; }
    
    [StringLength(200)]
    public string? SeoKeywords { get; set; }
    
    // Navigation properties
    public virtual ArticleCategory Category { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
}