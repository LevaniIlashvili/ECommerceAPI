using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public interface ICartService
    {
        Task<CartDTO> GetCartByUserId(int userId);
        Task<Result<CartItem>> AddToCart(int userId, int productId, int quantity);
        Task<Result<bool>> UpdateCartItem(int userId, int cartItemId, int quantity);
        Task<Result<bool>> RemoveCartItem(int userId, int cartItemId);
    }
}
