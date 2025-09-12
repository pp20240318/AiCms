using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly CmsDbContext _context;

    public CategoryService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.ArticleCategories
            .Include(c => c.Parent)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryTreeAsync()
    {
        var categories = await _context.ArticleCategories
            .Include(c => c.Parent)
            .Include(c => c.Children.Where(child => !child.IsDeleted))
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var rootCategories = categories.Where(c => c.ParentId == null).ToList();
        return rootCategories.Select(c => MapToDtoWithChildren(c, categories));
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.ArticleCategories
            .Include(c => c.Parent)
            .Include(c => c.Children.Where(child => !child.IsDeleted))
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (category == null) return null;

        return MapToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Check if name already exists at same level
        var existingCategory = await _context.ArticleCategories
            .Where(c => c.Name == request.Name && 
                       c.ParentId == request.ParentId && 
                       !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (existingCategory != null)
            throw new ArgumentException("同级分类名称已存在");

        // Validate parent exists if specified
        if (request.ParentId.HasValue)
        {
            var parent = await _context.ArticleCategories
                .Where(c => c.Id == request.ParentId && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (parent == null)
                throw new ArgumentException("父分类不存在");
        }

        var category = new ArticleCategory
        {
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder
        };

        _context.ArticleCategories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryByIdAsync(category.Id) ?? throw new Exception("创建分类失败");
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(UpdateCategoryRequest request)
    {
        var category = await _context.ArticleCategories
            .Where(c => c.Id == request.Id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (category == null) return null;

        // Check if name conflicts with other categories at same level
        var conflictCategory = await _context.ArticleCategories
            .Where(c => c.Id != request.Id && 
                       c.Name == request.Name && 
                       c.ParentId == request.ParentId && 
                       !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (conflictCategory != null)
            throw new ArgumentException("同级分类名称已存在");

        // Validate parent exists if specified
        if (request.ParentId.HasValue)
        {
            var parent = await _context.ArticleCategories
                .Where(c => c.Id == request.ParentId && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (parent == null)
                throw new ArgumentException("父分类不存在");

            // Prevent circular reference
            if (await IsCircularReference(request.Id, request.ParentId.Value))
                throw new ArgumentException("不能设置子分类为父分类");
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetCategoryByIdAsync(category.Id);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.ArticleCategories
            .Include(c => c.Children)
            .Include(c => c.Articles)
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (category == null) return false;

        // Check if category has children
        if (category.Children.Any(c => !c.IsDeleted))
            throw new ArgumentException("存在子分类，不能删除");

        // Check if category has articles
        if (category.Articles.Any())
            throw new ArgumentException("分类下存在文章，不能删除");

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private CategoryDto MapToDto(ArticleCategory category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt ?? category.CreatedAt
        };
    }

    private CategoryDto MapToDtoWithChildren(ArticleCategory category, List<ArticleCategory> allCategories)
    {
        var dto = MapToDto(category);
        dto.Children = category.Children
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(child => MapToDtoWithChildren(child, allCategories));
        return dto;
    }

    private async Task<bool> IsCircularReference(int categoryId, int parentId)
    {
        var current = await _context.ArticleCategories
            .Where(c => c.Id == parentId && !c.IsDeleted)
            .FirstOrDefaultAsync();

        while (current != null)
        {
            if (current.Id == categoryId)
                return true;

            if (current.ParentId == null)
                break;

            current = await _context.ArticleCategories
                .Where(c => c.Id == current.ParentId && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        return false;
    }
}