using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCms.Api.Models;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Summary { get; set; }
    
    public string? Description { get; set; }
    
    [StringLength(200)]
    public string? MainImage { get; set; }
    
    public string? Images { get; set; } // JSON array of image URLs
    
    public int CategoryId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? OriginalPrice { get; set; }
    
    [StringLength(50)]
    public string? Brand { get; set; }
    
    [StringLength(50)]
    public string? Model { get; set; }
    
    public int Stock { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsFeatured { get; set; } = false;
    
    public int SortOrder { get; set; } = 0;
    
    [StringLength(200)]
    public string? SeoTitle { get; set; }
    
    [StringLength(500)]
    public string? SeoDescription { get; set; }
    
    [StringLength(200)]
    public string? SeoKeywords { get; set; }
    
    // Navigation properties
    public virtual ProductCategory Category { get; set; } = null!;
}