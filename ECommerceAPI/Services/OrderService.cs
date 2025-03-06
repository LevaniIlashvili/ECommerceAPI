using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly CacheService _cacheService;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IMapper mapper, CacheService cacheService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<Result<OrderDTO>> CreateOrder(int userId, string shippingAddress)
        {
            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart.CartItems.Count == 0)
            {
                return Result<OrderDTO>.Failure("Cart is empty", ErrorType.BadRequest);
            }

            if (cart.CartItems.Any(ci => ci.Quantity > ci.Product.Stock))
            {
                return Result<OrderDTO>.Failure("One or more items in your cart are out of stock.", ErrorType.BadRequest);
            }

            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                OrderDate = DateTime.Now,
                Status = OrderStatus.Processing,
                ShippingAddress = shippingAddress,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()
            };

            var addedOrder = await _orderRepository.CreateOrder(order);

            await _cartRepository.UpdateStockAfterOrder(cart.CartItems);

            await _cartRepository.ClearCart(userId);

            await _cacheService.RemoveCacheByPrefixAsync("product");

            return Result<OrderDTO>.SuccessResult(_mapper.Map<OrderDTO>(addedOrder));
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserId(userId);

            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<Result<bool>> UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order == null)
            {
                return Result<bool>.Failure("Order doesn't exist", ErrorType.NotFound);
            }

            await _orderRepository.UpdateOrderStatus(orderId, orderStatus);
            return Result<bool>.SuccessResult(true);
        }
    }
}
