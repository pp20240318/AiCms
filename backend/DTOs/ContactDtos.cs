using System.ComponentModel.DataAnnotations;
using MyCms.Api.Models;

namespace MyCms.Api.DTOs
{
    public class ContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ContactStatus Status { get; set; }
        public string? Reply { get; set; }
        public DateTime? RepliedAt { get; set; }
        public int? RepliedById { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateContactDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Company { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateContactStatusDto
    {
        [Required]
        public ContactStatus Status { get; set; }

        public string? Reply { get; set; }
    }

    public class ContactQueryDto
    {
        public ContactStatus? Status { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}