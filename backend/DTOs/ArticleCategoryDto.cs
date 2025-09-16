namespace MyCms.Api.DTOs;

// Article Category DTOs
public class ArticleCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ArticleCategoryDto>? Children { get; set; }
    public int ArticleCount { get; set; }
}

public class CreateArticleCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class UpdateArticleCategoryDto : CreateArticleCategoryDto
{
    public int Id { get; set; }
}