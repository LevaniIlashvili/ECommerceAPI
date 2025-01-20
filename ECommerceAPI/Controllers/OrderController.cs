using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PlaceOrder(AddOrderDTO dto)
        {
            var userId = HttpContext.GetAuthenticatedUserId();

            var order = await _orderRepository.CreateOrder(userId, dto.ShippingAddress);

            var orderDTO = MapToOrderDTO(order);

            return Ok(orderDTO);
        }

        [HttpPut("{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            await _orderRepository.UpdateOrderStatus(orderId, orderStatus);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUserId()
        {
            var userId = HttpContext.GetAuthenticatedUserId();

            var orders = await _orderRepository.GetOrdersByUserId(userId);

            var orderDTOs = orders.Select(o => MapToOrderDTO(o));

            return Ok(orderDTOs);
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
