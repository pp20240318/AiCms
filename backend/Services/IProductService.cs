using MyCms.Api.DTOs;

namespace MyCms.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductListRequest request);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> ToggleActiveAsync(int id);
    Task<bool> ToggleFeaturedAsync(int id);
}

public interface IProductCategoryService
{
    Task<List<ProductCategoryDto>> GetCategoriesAsync(bool includeInactive = false);
    Task<ProductCategoryDto?> GetCategoryByIdAsync(int id);
    Task<ProductCategoryDto> CreateCategoryAsync(CreateProductCategoryRequest request);
    Task<ProductCategoryDto?> UpdateCategoryAsync(UpdateProductCategoryRequest request);
    Task<bool> DeleteCategoryAsync(int id);
    Task<List<ProductCategoryDto>> GetCategoryTreeAsync();
}