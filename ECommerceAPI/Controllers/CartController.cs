using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }


    [HttpGet]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        var cart = await _cartService.GetCartByUserId(userId);

        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddToCart(AddCartItemDTO dto)
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        var result = await _cartService.AddToCart(userId, dto.ProductId, dto.Quantity);
        
        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                _ => StatusCode(500, new { Message = "An unexpected error occurred." })
            };
        }

        bool isNewItem = result.Data!.Quantity == dto.Quantity;

        if (isNewItem)
        {
            return CreatedAtAction(nameof(GetCart), new { userId }, new { Message = "Product added to cart successfully." });
        }

        return Ok(new { Message = "Product quantity updated in cart successfully." });
    }

    [HttpPut("update/{cartItemId}")]
    public async Task<ActionResult> UpdateCartItem(int cartItemId, UpdateCartItemDTO dto)
    {
        var userId = HttpContext.GetAuthenticatedUserId();
        var result = await _cartService.UpdateCartItem(userId, cartItemId, dto.Quantity);

        if (!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ErrorType.Forbid => Forbid(),
                _ => StatusCode(500, new { Message = "An unexpected error occurred." })
            };
        }

        return Ok(new { Message = "Cart item updated successfully." });
    }

    [HttpDelete("delete/{cartItemId}")]
    public async Task<ActionResult> RemoveCartItem(int cartItemId)
    {
        var userId = HttpContext.GetAuthenticatedUserId();

        var result = await _cartService.RemoveCartItem(userId, cartItemId);

        if(!result.Success)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ErrorType.Unauthorized => Unauthorized(new { Message = result.ErrorMessage }),
                _ => StatusCode(500, new { Message = "An unexpected error occurred. - from controller" })
            };
        }

        return Ok(new { Message = "Cart item removed successfully." });
    }
}