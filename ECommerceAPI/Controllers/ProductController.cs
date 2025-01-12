using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
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
        public async Task<ActionResult<Product>> GetProduct(int id)
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
        public async Task<ActionResult> AddProduct(Product product)
        {
            try
            {
                var category = await _categoryRepository.GetCategory(product.CategoryId);
                if (category == null)
                {
                    return BadRequest("Invalid CategoryId. The specified category does not exist.");
                }
                await _productRepository.AddProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occurred while adding the product.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            try
            {
                if (id != updatedProduct.Id)
                {
                    return BadRequest("Product ID mismatch");
                }

                var category = await _categoryRepository.GetCategory(updatedProduct.CategoryId);
                if (category == null)
                {
                    return BadRequest("Invalid CategoryId. The specified category does not exist.");
                }

                var existingProduct = await _productRepository.GetProduct(id);
                if (existingProduct == null)
                {
                    return NotFound(new { Message = $"Product with Id = {id} not found" });
                }

                await _productRepository.UpdateProduct(updatedProduct);

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
