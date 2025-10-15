using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCms.Api.DTOs;
using MyCms.Api.Services;

namespace MyCms.Api.Areas.Public.Controllers
{
    [Area("Public")]
    [Route("api/public/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CategoriesController : ControllerBase
    {
        private readonly IArticleCategoryService _categoryService;

        public CategoriesController(IArticleCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// 获取所有文章分类（公开接口）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ArticleCategoryDto>>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                return Ok(ApiResponse<List<ArticleCategoryDto>>.SuccessResult(categories));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ArticleCategoryDto>>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// 根据ID获取分类信息（公开接口）
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ArticleCategoryDto>>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryAsync(id);
                if (category == null)
                {
                    return NotFound(ApiResponse<ArticleCategoryDto>.ErrorResult("分类不存在"));
                }

                return Ok(ApiResponse<ArticleCategoryDto>.SuccessResult(category));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ArticleCategoryDto>.ErrorResult(ex.Message));
            }
        }
    }
}