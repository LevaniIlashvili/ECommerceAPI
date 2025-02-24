using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<CartDTO> GetCartByUserId(int userId)
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<Result<CartItem>> AddToCart(int userId, int productId, int quantity)
        {
            var product = await _productRepository.GetProduct(productId);
            if (product == null)
            {
                return Result<CartItem>.Failure("Product not found", ErrorType.NotFound);
            }

            if (product.Stock < quantity)
            {
                return Result<CartItem>.Failure("Insufficient stock", ErrorType.BadRequest);
            }

            var cart = await _cartRepository.GetCartByUserId(userId);
             
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                if (product.Stock < quantity + cartItem.Quantity)
                {
                    return Result<CartItem>.Failure("Insufficient stock", ErrorType.BadRequest);
                }
                await _cartRepository.UpdateCartItem(cartItem.Id, cartItem.Quantity + quantity);
            }
            else
            {
                cartItem = await _cartRepository.AddToCart(userId, productId, quantity);
            }

            return Result<CartItem>.SuccessResult(cartItem);
        }

        public async Task<Result<bool>> UpdateCartItem(int userId, int cartItemId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return Result<bool>.Failure("Cart item not found", ErrorType.NotFound);
            }

            var product = await _productRepository.GetProduct(cartItem.ProductId);
            if (product == null)
            {
                return Result<bool>.Failure("Product not found", ErrorType.NotFound);
            }

            if (product.Stock < quantity)
            {
                return Result<bool>.Failure("Insufficient stock", ErrorType.BadRequest);
            }

            await _cartRepository.UpdateCartItem(cartItemId, quantity);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> RemoveCartItem(int userId, int cartItemId)
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return Result<bool>.Failure("Cart item not found", ErrorType.NotFound);
            }

            await _cartRepository.RemoveCartItem(cartItemId);

            return Result<bool>.SuccessResult(true);
        }

    }
}
