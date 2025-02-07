using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryRepository.GetCategories();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetCategory(id);

            if (category == null)
            {
                return NotFound(new { Message = $"Category with Id = {id} not found" });
            }

            return Ok(category);
        }


        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(AddCategoryDTO categoryDTO)
        {
            var result = await _categoryRepository.AddCategory(categoryDTO.Name);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
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
            var result = await _categoryRepository.UpdateCategory(id, categoryDTO.Name);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _categoryRepository.DeleteCategory(id);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }
    }
}