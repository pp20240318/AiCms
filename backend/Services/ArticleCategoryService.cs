using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public class ArticleCategoryService : IArticleCategoryService
{
    private readonly CmsDbContext _context;

    public ArticleCategoryService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ArticleCategoryDto>> GetCategoriesAsync(bool includeInactive = false)
    {
        var query = _context.ArticleCategories.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new ArticleCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ParentId = c.ParentId,
                ParentName = c.Parent != null ? c.Parent.Name : null,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
                ArticleCount = c.Articles.Count
            })
            .ToListAsync();
    }

    public async Task<ArticleCategoryDto?> GetCategoryAsync(int id)
    {
        var category = await _context.ArticleCategories
            .Include(c => c.Parent)
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new ArticleCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt ?? category.CreatedAt,
            ArticleCount = category.Articles.Count
        };
    }

    public async Task<ArticleCategoryDto> CreateCategoryAsync(CreateArticleCategoryDto request)
    {
        var category = new ArticleCategory
        {
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        _context.ArticleCategories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryAsync(category.Id) ?? throw new Exception("创建分类失败");
    }

    public async Task<ArticleCategoryDto?> UpdateCategoryAsync(int id, UpdateArticleCategoryDto request)
    {
        var category = await _context.ArticleCategories.FindAsync(id);
        if (category == null) return null;

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetCategoryAsync(category.Id);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.ArticleCategories
            .Include(c => c.Children)
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return false;

        // Check if category has children or articles
        if (category.Children.Any() || category.Articles.Any())
        {
            throw new InvalidOperationException("Cannot delete category with children or articles");
        }

        _context.ArticleCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ArticleCategoryDto>> GetCategoryTreeAsync()
    {
        var allCategories = await _context.ArticleCategories
            .Where(c => c.IsActive)
            .Include(c => c.Articles)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var categoryDtos = allCategories.Select(c => new ArticleCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ParentId = c.ParentId,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
            ArticleCount = c.Articles.Count,
            Children = new List<ArticleCategoryDto>()
        }).ToList();

        var rootCategories = categoryDtos.Where(c => c.ParentId == null).ToList();
        BuildCategoryTree(rootCategories, categoryDtos);

        return rootCategories;
    }

    public async Task<ArticleCategoryDto?> ToggleStatusAsync(int id)
    {
        var category = await _context.ArticleCategories.FindAsync(id);
        if (category == null) return null;

        category.IsActive = !category.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetCategoryAsync(category.Id);
    }

    private void BuildCategoryTree(List<ArticleCategoryDto> parentCategories, List<ArticleCategoryDto> allCategories)
    {
        foreach (var parent in parentCategories)
        {
            parent.Children = allCategories.Where(c => c.ParentId == parent.Id).ToList();
            if (parent.Children.Any())
            {
                BuildCategoryTree(parent.Children, allCategories);
            }
        }
    }
}