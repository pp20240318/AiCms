using System.ComponentModel.DataAnnotations;

namespace MyCms.Api.DTOs
{
    public class WebsiteConfigDto
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Description { get; set; }
        public string Group { get; set; } = "general";
        public string DataType { get; set; } = "string";
        public bool IsPublic { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateWebsiteConfigDto
    {
        [Required]
        [MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        public string? Value { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Group { get; set; } = "general";

        [MaxLength(20)]
        public string DataType { get; set; } = "string";

        public bool IsPublic { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }

    public class UpdateWebsiteConfigDto
    {
        public string? Value { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Group { get; set; } = "general";

        [MaxLength(20)]
        public string DataType { get; set; } = "string";

        public bool IsPublic { get; set; } = false;

        public int SortOrder { get; set; } = 0;
    }

    public class PublicConfigDto
    {
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string DataType { get; set; } = "string";
    }

    public class WebsiteConfigQueryDto
    {
        public string? Group { get; set; }
        public bool? IsPublic { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}