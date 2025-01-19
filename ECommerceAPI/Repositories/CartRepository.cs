using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
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

        public async Task<CartDTO?> GetCartByUserId(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new CartDTO
                {
                    UserId = c.UserId,
                    CartItems = c.CartItems.Select(ci => new CartItemDTO
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task AddToCart(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found.");
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

            if (product.Stock < quantity)
                throw new ArgumentException($"Insufficient stock for product {productId}.");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                if (product.Stock < quantity + cartItem.Quantity)
                    throw new ArgumentException($"Insufficient stock for product {productId}.");
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetCartItemById(int cartItemId, int userId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);
        }

        public async Task UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (cartItem == null) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");

            if (cartItem.Cart.UserId != userId) throw new UnauthorizedAccessException("User is not authorized");

            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product not found.");

            if (product.Stock < quantity)
                throw new ArgumentException($"Insufficient stock for product {product.Id}.");

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItem(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (cartItem == null) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");

            if (cartItem.Cart.UserId != userId) throw new UnauthorizedAccessException("User is not authorized");

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }


    }
}
