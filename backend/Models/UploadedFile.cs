using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class UploadedFile : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string OriginalName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string FilePath { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string ContentType { get; set; } = string.Empty;
    
    public long FileSize { get; set; }
    
    [StringLength(32)]
    public string? FileHash { get; set; }
    
    public int? Width { get; set; }
    
    public int? Height { get; set; }
    
    public int? UserId { get; set; }
    
    // Navigation properties
    public virtual User? User { get; set; }
}