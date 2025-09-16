using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IArticleCategoryService
{
    Task<List<ArticleCategoryDto>> GetCategoriesAsync(bool includeInactive = false);
    Task<ArticleCategoryDto?> GetCategoryAsync(int id);
    Task<ArticleCategoryDto> CreateCategoryAsync(CreateArticleCategoryDto request);
    Task<ArticleCategoryDto?> UpdateCategoryAsync(int id, UpdateArticleCategoryDto request);
    Task<bool> DeleteCategoryAsync(int id);
    Task<List<ArticleCategoryDto>> GetCategoryTreeAsync();
    Task<ArticleCategoryDto?> ToggleStatusAsync(int id);
}