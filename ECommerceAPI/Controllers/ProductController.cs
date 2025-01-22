using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = new Category
                    {
                        Id = p.Category!.Id,
                        Name = p.Category.Name
                    }
                });
                return Ok(productDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while retrieving products.");
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProduct(id);
                if (product == null)
                {
                    return NotFound(new { Message = $"Product with Id = {id} not found" });
                }
                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Category = new Category
                    {
                        Id = product.Category!.Id,
                        Name = product.Category.Name
                    }
                };
                return Ok(productDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "An error occurred while retrieving the product.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(AddProductDTO productDTO)
        {
            try
            {
                var category = await _categoryRepository.GetCategory(productDTO.CategoryId);
                if (category == null)
                {
                    return BadRequest("Invalid CategoryId. The specified category does not exist.");
                }
                var product = await _productRepository.AddProduct(productDTO);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while adding the product.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, AddProductDTO productDTO)
        {
            try
            {
                var category = await _categoryRepository.GetCategory(productDTO.CategoryId);
                if (category == null)
                {
                    return BadRequest("Invalid CategoryId. The specified category does not exist.");
                }

                var existingProduct = await _productRepository.GetProduct(id);
                if (existingProduct == null)
                {
                    return NotFound(new { Message = $"Product with Id = {id} not found" });
                }

                await _productRepository.UpdateProduct(id, productDTO);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while updating the product.");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProduct(id);
                if (product == null)
                {
                    return NotFound(new { Message = $"Product with Id = {id} not found" });
                }

                await _productRepository.DeleteProduct(id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while deleting the product.");
            }
        }
    }
}
