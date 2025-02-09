using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
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
                .AsNoTracking()
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

        public async Task<RepositoryResult<CartItem>> AddToCart(int userId, int productId, int quantity)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RepositoryResult<CartItem>.Failure("Invalid user ID", RepositoryErrorType.BadRequest);
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return RepositoryResult<CartItem>.Failure("Product not found", RepositoryErrorType.NotFound);
            }

            if (product.Stock < quantity)
            {
                return RepositoryResult<CartItem>.Failure("Insufficient stock", RepositoryErrorType.BadRequest);
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                if (product.Stock < quantity + cartItem.Quantity)
                {
                    return RepositoryResult<CartItem>.Failure("Insufficient stock", RepositoryErrorType.BadRequest);
                }
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };
                cart.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return RepositoryResult<CartItem>.SuccessResult(cartItem);
        }

        public async Task<RepositoryResult<bool>> UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return RepositoryResult<bool>.Failure($"Cart item with ID {cartItemId} not found.", RepositoryErrorType.NotFound);
            }

            if (cartItem.Cart.UserId != userId)
            {
                return RepositoryResult<bool>.Failure("User is not authorized to modify this cart item.", RepositoryErrorType.Forbid);
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product == null)
            {
                return RepositoryResult<bool>.Failure($"Product item with ID {cartItem.ProductId} not found.", RepositoryErrorType.NotFound);
            }

            if (product.Stock < quantity)
            {
                return RepositoryResult<bool>.Failure($"Insufficient stock for product {product.Id}", RepositoryErrorType.BadRequest);
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return RepositoryResult<bool>.SuccessResult(true);
        }

        public async Task<RepositoryResult<bool>> RemoveCartItem(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return RepositoryResult<bool>.Failure($"Cart item with ID {cartItemId} not found.", RepositoryErrorType.NotFound);
            }

            if (cartItem.Cart.UserId != userId)
            {
                return RepositoryResult<bool>.Failure("User is not authorized to modify this cart item.", RepositoryErrorType.Unauthorized);
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.SuccessResult(true);
        }


    }
}
