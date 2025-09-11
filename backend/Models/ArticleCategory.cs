using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class ArticleCategory : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public int? ParentId { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ArticleCategory? Parent { get; set; }
    public virtual ICollection<ArticleCategory> Children { get; set; } = new List<ArticleCategory>();
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}