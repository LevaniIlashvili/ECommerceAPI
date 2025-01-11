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

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetProducts();
                return Ok(products);
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
                return Ok(product);
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
                await _productRepository.AddProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while adding the product.");
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
