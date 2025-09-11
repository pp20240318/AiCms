using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class User : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? RealName { get; set; }
    
    [StringLength(20)]
    public string? Phone { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}