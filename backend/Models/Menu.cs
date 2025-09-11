using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class Menu : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Path { get; set; }
    
    [StringLength(50)]
    public string? Icon { get; set; }
    
    public int? ParentId { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsVisible { get; set; } = true;
    
    [StringLength(50)]
    public string? Permission { get; set; }
    
    // Navigation properties
    public virtual Menu? Parent { get; set; }
    public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
}