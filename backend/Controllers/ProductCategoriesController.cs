using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoriesController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetCategories(
        [FromQuery] bool includeInactive = false)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesAsync(includeInactive);
            return Ok(ApiResponse<List<ProductCategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ProductCategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<ProductCategoryDto>>>> GetCategoryTree()
    {
        try
        {
            var tree = await _categoryService.GetCategoryTreeAsync();
            return Ok(ApiResponse<List<ProductCategoryDto>>.SuccessResult(tree));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ProductCategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<ProductCategoryDto>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse<ProductCategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> CreateCategory([FromBody] CreateProductCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id },
                ApiResponse<ProductCategoryDto>.SuccessResult(category, "分类创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ProductCategoryDto>>> UpdateCategory(int id, [FromBody] CreateProductCategoryRequest request)
    {
        try
        {
            var updateRequest = new UpdateProductCategoryRequest
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Image = request.Image,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            var category = await _categoryService.UpdateCategoryAsync(updateRequest);
            if (category == null)
            {
                return NotFound(ApiResponse<ProductCategoryDto>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse<ProductCategoryDto>.SuccessResult(category, "分类更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ProductCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(int id)
    {
        try
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "分类删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}