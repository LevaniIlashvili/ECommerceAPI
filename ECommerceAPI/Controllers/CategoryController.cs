using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { Message = $"Category with Id {id} not found" });
            }

            return Ok(category);
        }


        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(AddCategoryDTO categoryDTO)
        {
            var result = await _categoryService.AddCategoryAsync(categoryDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            if (result.Data == null)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCategory(int id, AddCategoryDTO categoryDTO)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    ErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }
    }
}