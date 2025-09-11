using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class ScheduledTask : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string CronExpression { get; set; } = string.Empty;
    
    public string? Parameters { get; set; } // JSON parameters
    
    public bool IsEnabled { get; set; } = true;
    
    public DateTime? NextRunTime { get; set; }
    
    public DateTime? LastRunTime { get; set; }
    
    [StringLength(50)]
    public string? LastRunStatus { get; set; }
    
    public string? LastRunResult { get; set; }
    
    public int RunCount { get; set; } = 0;
}