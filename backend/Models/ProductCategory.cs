using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class ProductCategory : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    public string? Image { get; set; }
    
    public int? ParentId { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ProductCategory? Parent { get; set; }
    public virtual ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}