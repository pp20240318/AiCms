using Microsoft.EntityFrameworkCore;
using MyCms.Api.Data;
using MyCms.Api.DTOs;
using MyCms.Api.Models;
using System.Text.Json;

namespace MyCms.Api.Services;

public class ProductService : IProductService
{
    private readonly CmsDbContext _context;

    public ProductService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductListRequest request)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => p.Name.Contains(request.Search) ||
                                   p.Summary!.Contains(request.Search) ||
                                   p.Brand!.Contains(request.Search));
        }

        // Category filter
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId);
        }

        // Active filter
        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive);
        }

        // Featured filter
        if (request.IsFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == request.IsFeatured);
        }

        // Brand filter
        if (!string.IsNullOrEmpty(request.Brand))
        {
            query = query.Where(p => p.Brand == request.Brand);
        }

        // Price range filter
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderBy(p => p.SortOrder)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Summary = p.Summary,
                Description = p.Description,
                MainImage = p.MainImage,
                Images = new List<string>(),
                CategoryId = p.CategoryId,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                Brand = p.Brand,
                Model = p.Model,
                Stock = p.Stock,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                SortOrder = p.SortOrder,
                SeoTitle = p.SeoTitle,
                SeoDescription = p.SeoDescription,
                SeoKeywords = p.SeoKeywords,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt ?? p.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Summary = product.Summary,
            Description = product.Description,
            MainImage = product.MainImage,
            Images = new List<string>(),

            CategoryId = product.CategoryId,
            Brand = product.Brand,
            Model = product.Model,
            Stock = product.Stock,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            SortOrder = product.SortOrder,
            SeoTitle = product.SeoTitle,
            SeoDescription = product.SeoDescription,
            SeoKeywords = product.SeoKeywords,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt ?? product.CreatedAt
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Summary = request.Summary,
            Description = request.Description,
            MainImage = request.MainImage,
            Images = request.Images != null && request.Images.Any() ? 
                JsonSerializer.Serialize(request.Images) : null,
            CategoryId = request.CategoryId,
            Price = request.Price,
            OriginalPrice = request.OriginalPrice,
            Brand = request.Brand,
            Model = request.Model,
            Stock = request.Stock,
            IsActive = request.IsActive,
            IsFeatured = request.IsFeatured,
            SortOrder = request.SortOrder,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            SeoKeywords = request.SeoKeywords
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id) ?? throw new Exception("创建产品失败");
    }

    public async Task<ProductDto?> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null) return null;

        product.Name = request.Name;
        product.Summary = request.Summary;
        product.Description = request.Description;
        product.MainImage = request.MainImage;
        product.Images = request.Images != null && request.Images.Any() ? 
            JsonSerializer.Serialize(request.Images) : null;
        product.CategoryId = request.CategoryId;
        product.Price = request.Price;
        product.OriginalPrice = request.OriginalPrice;
        product.Brand = request.Brand;
        product.Model = request.Model;
        product.Stock = request.Stock;
        product.IsActive = request.IsActive;
        product.IsFeatured = request.IsFeatured;
        product.SortOrder = request.SortOrder;
        product.SeoTitle = request.SeoTitle;
        product.SeoDescription = request.SeoDescription;
        product.SeoKeywords = request.SeoKeywords;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetProductByIdAsync(product.Id);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFeaturedAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.IsFeatured = !product.IsFeatured;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}

public class ProductCategoryService : IProductCategoryService
{
    private readonly CmsDbContext _context;

    public ProductCategoryService(CmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductCategoryDto>> GetCategoriesAsync(bool includeInactive = false)
    {
        var query = _context.ProductCategories.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new ProductCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Image = c.Image,
                ParentId = c.ParentId,
                ParentName = c.Parent != null ? c.Parent.Name : null,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<ProductCategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.ProductCategories
            .Include(c => c.Parent)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return new ProductCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Image = category.Image,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt ?? category.CreatedAt,
            ProductCount = category.Products.Count
        };
    }

    public async Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequest request)
    {
        var category = new ProductCategory
        {
            Name = request.Name,
            Description = request.Description,
            Image = request.Image,
            ParentId = request.ParentId,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();

        return await GetCategoryByIdAsync(category.Id) ?? throw new Exception("创建分类失败");
    }

    public async Task<ProductCategoryDto?> UpdateCategoryAsync(UpdateProductCategoryRequest request)
    {
        var category = await _context.ProductCategories.FindAsync(request.Id);
        if (category == null) return null;

        category.Name = request.Name;
        category.Description = request.Description;
        category.Image = request.Image;
        category.ParentId = request.ParentId;
        category.SortOrder = request.SortOrder;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetCategoryByIdAsync(category.Id);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.ProductCategories
            .Include(c => c.Children)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return false;

        // Check if category has children or products
        if (category.Children.Any() || category.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete category with children or products");
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductCategoryDto>> GetCategoryTreeAsync()
    {
        var allCategories = await _context.ProductCategories
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var categoryDtos = allCategories.Select(c => new ProductCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Image = c.Image,
            ParentId = c.ParentId,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
            ProductCount = c.Products.Count,
            Children = new List<ProductCategoryDto>()
        }).ToList();

        var rootCategories = categoryDtos.Where(c => c.ParentId == null).ToList();
        BuildCategoryTree(rootCategories, categoryDtos);

        return rootCategories;
    }

    private void BuildCategoryTree(List<ProductCategoryDto> parentCategories, List<ProductCategoryDto> allCategories)
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