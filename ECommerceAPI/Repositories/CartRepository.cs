using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreDbContext _context;

        public CartRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserId(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstAsync(c => c.UserId == userId);
        }

        public async Task<CartItem> AddToCart(int userId, int productId, int quantity)
        {
            var cart = await GetCartByUserId(userId);

            var cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };

            cart.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<bool> UpdateCartItem(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FirstAsync(ci => ci.Id == cartItemId);

            cartItem.Quantity = quantity;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FirstAsync(ci => ci.Id == cartItemId);

            _context.CartItems.Remove(cartItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClearCart(int userId)
        {
            var cart = await _context.Carts.FirstAsync(c => c.UserId == userId);

            _context.CartItems.RemoveRange(cart.CartItems);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStockAfterOrder(List<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                var product = await _context.Products.FirstAsync(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
