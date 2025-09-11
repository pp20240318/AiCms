using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class Banner : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string ImageUrl { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? LinkUrl { get; set; }
    
    [StringLength(20)]
    public string? LinkTarget { get; set; } = "_blank";
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public int SortOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? StartTime { get; set; }
    
    public DateTime? EndTime { get; set; }
}