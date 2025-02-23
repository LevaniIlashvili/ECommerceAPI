using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
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
                Status = OrderStatus.Pending,
                ShippingAddress = shippingAddress,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()
            };

            var addedOrder = await _orderRepository.CreateOrder(order);

            return Result<OrderDTO>.SuccessResult(MapToOrderDTO(addedOrder));
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserId(userId);

            var orderDTOs = orders.Select(o => MapToOrderDTO(o));

            return orderDTOs;
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

        private static OrderDTO MapToOrderDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            };
        }
    }
}
