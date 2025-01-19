using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }


    [HttpGet("")]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        try
        {
            var cart = await _cartRepository.GetCartByUserId(userId);
            if (cart == null) return NotFound(new { Message = "Cart not found for the user." });

            return Ok(cart);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddToCart(AddCartItemDTO dto)
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        try
        {
            await _cartRepository.AddToCart(userId, dto.ProductId, dto.Quantity);
            return Ok(new { Message = "Product added to cart successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("update/{cartItemId}")]
    public async Task<ActionResult> UpdateCartItem(int cartItemId, UpdateCartItemDTO dto)
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        try
        {
            await _cartRepository.UpdateCartItem(userId, cartItemId, dto.Quantity);
            return Ok(new { Message = "Cart item updated successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("delete/{cartItemId}")]
    public async Task<ActionResult> RemoveCartItem(int cartItemId)
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        try
        {
            await _cartRepository.RemoveCartItem(userId, cartItemId);
            return Ok(new { Message = "Cart item removed successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message );
        }
    }
}