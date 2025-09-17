using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.Models;

public class Member : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string MemberCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? IdNumber { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string MembershipType { get; set; } = "Regular";

    [StringLength(20)]
    public string Status { get; set; } = "Active";

    public DateTime JoinDate { get; set; } = DateTime.Now;

    public DateTime? ExpiryDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(200)]
    public string? Avatar { get; set; }

    [StringLength(50)]
    public string? Occupation { get; set; }

    [StringLength(100)]
    public string? Company { get; set; }

    [StringLength(50)]
    public string? EmergencyContact { get; set; }

    [StringLength(20)]
    public string? EmergencyPhone { get; set; }

    public decimal? Balance { get; set; } = 0;

    public int Points { get; set; } = 0;

    public DateTime? LastVisitDate { get; set; }

    [StringLength(50)]
    public string? ReferralCode { get; set; }

    [StringLength(50)]
    public string? ReferredBy { get; set; }
}