using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticleCategoriesController : ControllerBase
{
    private readonly IArticleCategoryService _categoryService;

    public ArticleCategoriesController(IArticleCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ArticleCategoryDto>>>> GetCategories(
        [FromQuery] bool includeInactive = false)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesAsync(includeInactive);
            return Ok(ApiResponse<List<ArticleCategoryDto>>.SuccessResult(categories));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ArticleCategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResponse<List<ArticleCategoryDto>>>> GetCategoryTree()
    {
        try
        {
            var tree = await _categoryService.GetCategoryTreeAsync();
            return Ok(ApiResponse<List<ArticleCategoryDto>>.SuccessResult(tree));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ArticleCategoryDto>>.ErrorResult(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ArticleCategoryDto>>> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryAsync(id);
            if (category == null)
                return NotFound(ApiResponse<ArticleCategoryDto>.ErrorResult("分类不存在"));

            return Ok(ApiResponse<ArticleCategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ArticleCategoryDto>>> CreateCategory(CreateArticleCategoryDto dto)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id },
                ApiResponse<ArticleCategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ArticleCategoryDto>>> UpdateCategory(int id, UpdateArticleCategoryDto dto)
    {
        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            if (category == null)
                return NotFound(ApiResponse<ArticleCategoryDto>.ErrorResult("分类不存在"));

            return Ok(ApiResponse<ArticleCategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleCategoryDto>.ErrorResult(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResult("分类不存在或已被使用"));

            return Ok(ApiResponse<bool>.SuccessResult(true, "分类删除成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("{id}/toggle")]
    public async Task<ActionResult<ApiResponse<ArticleCategoryDto>>> ToggleStatus(int id)
    {
        try
        {
            var category = await _categoryService.ToggleStatusAsync(id);
            if (category == null)
                return NotFound(ApiResponse<ArticleCategoryDto>.ErrorResult("分类不存在"));

            return Ok(ApiResponse<ArticleCategoryDto>.SuccessResult(category));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ArticleCategoryDto>.ErrorResult(ex.Message));
        }
    }
}