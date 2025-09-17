using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.DTOs;

public class CreateMemberDto
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

    [StringLength(50)]
    public string? ReferralCode { get; set; }

    [StringLength(50)]
    public string? ReferredBy { get; set; }
}

public class UpdateMemberDto
{
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

    public decimal? Balance { get; set; }

    public int Points { get; set; }

    [StringLength(50)]
    public string? ReferralCode { get; set; }

    [StringLength(50)]
    public string? ReferredBy { get; set; }
}

public class MemberDto
{
    public int Id { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? IdNumber { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public string? Avatar { get; set; }
    public string? Occupation { get; set; }
    public string? Company { get; set; }
    public string? EmergencyContact { get; set; }
    public string? EmergencyPhone { get; set; }
    public decimal? Balance { get; set; }
    public int Points { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public string? ReferralCode { get; set; }
    public string? ReferredBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class MemberListDto
{
    public int Id { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string MembershipType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? Balance { get; set; }
    public int Points { get; set; }
}