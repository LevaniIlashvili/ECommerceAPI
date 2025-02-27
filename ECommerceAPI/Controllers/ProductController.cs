using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PaginationResultDTO<ProductDTO>>> GetProducts([FromQuery] string? category = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productService.GetAllProductsAsync(category, pageNumber, pageSize);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var productDTO = await _productService.GetProductByIdAsync(id);

            if (productDTO == null)
            {
                return NotFound(new { Message = $"Product not found" });
            }
  
            return Ok(productDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> AddProduct(AddProductDTO addProductDTO)
        {
            var result = await _productService.AddProductAsync(addProductDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    ErrorType.Conflict => Conflict(new {Message = result.ErrorMessage}),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, AddProductDTO productDTO)
        {
            var result = await _productService.UpdateProductAsync(id, productDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    ErrorType.BadRequest => BadRequest(new {Message = result.ErrorMessage}),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return Ok(new { Message = "Product updated successfully." });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

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
    }
}
