using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserId(int userId);
        Task<CartItem> AddToCart(int userId, int productId, int quantity);
        Task<bool> UpdateCartItem(int cartItemId, int quantity);
        Task<bool> RemoveCartItem(int cartItemId);
        Task<bool> ClearCart(int userId);
        Task<bool> UpdateStockAfterOrder(List<CartItem> cartItems);
    }
}
