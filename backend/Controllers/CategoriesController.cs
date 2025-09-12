using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategoryTree()
    {
        try
        {
            var categories = await _categoryService.GetCategoryTreeAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<CategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id },
                ApiResponse<CategoryDto>.SuccessResult(category, "分类创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
    {
        try
        {
            var updateRequest = new UpdateCategoryRequest
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder
            };

            var category = await _categoryService.UpdateCategoryAsync(updateRequest);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResult(category, "分类更新成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(int id)
    {
        try
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResult("分类不存在"));
            }

            return Ok(ApiResponse.SuccessResult("分类删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
        }
    }
}