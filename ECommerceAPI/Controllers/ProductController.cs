using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
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
            var products = await _productRepository.GetProducts();
            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Category = p.Category!
            });
            return Ok(productDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound(new { Message = $"Product not found" });
            }
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category!
            };
            return Ok(productDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> AddProduct(AddProductDTO addProductDTO)
        {
            var result = await _productRepository.AddProduct(addProductDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    RepositoryErrorType.Conflict => Conflict(new {Message = result.ErrorMessage}),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            if (result.Data == null)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }

            var productDto = new ProductDto
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                Price = result.Data.Price,
                Stock = result.Data.Stock,
                Category = result.Data.Category!
            };
            return CreatedAtAction(nameof(GetProduct), new { id = result.Data.Id }, productDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, AddProductDTO productDTO)
        {
            var result = await _productRepository.UpdateProduct(id, productDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    RepositoryErrorType.BadRequest => BadRequest(new {Message = result.ErrorMessage}),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productRepository.DeleteProduct(id);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    RepositoryErrorType.Conflict => Conflict(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }
    }
}
