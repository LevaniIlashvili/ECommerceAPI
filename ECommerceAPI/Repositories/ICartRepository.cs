using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface ICartRepository
    {
        Task<CartDTO?> GetCartByUserId(int userId);
        Task<RepositoryResult<CartItem>> AddToCart(int userId, int productId, int quantity);
        Task<RepositoryResult<bool>> UpdateCartItem(int userId, int cartItemId, int quantity);
        Task<RepositoryResult<bool>> RemoveCartItem(int userId, int cartItemId);
    }
}
