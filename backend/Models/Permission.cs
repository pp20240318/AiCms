using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class Permission : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string? Module { get; set; }
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}