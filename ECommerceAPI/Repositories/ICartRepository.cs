using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDTO?> GetCartByUserId(int userId);
        Task AddToCart(int userId, int productId, int quantity);
        Task UpdateCartItem(int userId, int cartItemId, int quantity);
        Task RemoveCartItem(int userId, int cartItemId);
    }
}
