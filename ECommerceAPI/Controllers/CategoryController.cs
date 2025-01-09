using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategories();
                return Ok(categories);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving categories.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategory(id);

                if (category == null)
                {
                    return NotFound(new { Message = $"Category with Id = {id} not found" });
                }

                return Ok(category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "An error occurred while retrieving the category.");
            }
        }


        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            try
            {
                await _categoryRepository.AddCategory(category);

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while adding the category.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCategory(int id, Category updatedCategory)
        {
            try
            {
                if (id != updatedCategory.Id)
                {
                    return BadRequest("Category ID mismatch");
                }

                var existingCategory = await _categoryRepository.GetCategory(id);
                if (existingCategory == null)
                {
                    return NotFound(new { Message = $"Category with Id = {id} not found" });
                }

                await _categoryRepository.UpdateCategory(updatedCategory);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the category.");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategory(id);
                if (category == null)
                {
                    return NotFound(new { Message = $"Category with Id = {id} not found" });
                }

                await _categoryRepository.DeleteCategory(id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the category.");
            }
        }
    }
}
