using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] string? brand = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            var request = new ProductListRequest
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                CategoryId = categoryId,
                IsActive = isActive,
                IsFeatured = isFeatured,
                Brand = brand,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var result = await _productService.GetProductsAsync(request);
            return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedResult<ProductDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResult("产品不存在"));
            }

            return Ok(ApiResponse<ProductDto>.SuccessResult(product));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id },
                ApiResponse<ProductDto>.SuccessResult(product, "产品创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, [FromBody] CreateProductRequest request)
    {
        try
        {
            var updateRequest = new UpdateProductRequest
            {
                Id = id,
                Name = request.Name,
                Summary = request.Summary,
                Description = request.Description,
                MainImage = request.MainImage,
                Images = request.Images,
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

            var product = await _productService.UpdateProductAsync(updateRequest);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResult("产品不存在"));
            }

            return Ok(ApiResponse<ProductDto>.SuccessResult(product, "产品更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
    {
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("产品不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "产品删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPatch("{id}/toggle-active")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ToggleActive(int id)
    {
        try
        {
            var success = await _productService.ToggleActiveAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("产品不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "产品状态切换成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }

    [HttpPatch("{id}/toggle-featured")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ToggleFeatured(int id)
    {
        try
        {
            var success = await _productService.ToggleFeaturedAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("产品不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "推荐状态切换成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}