using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
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

            var result = await _orderRepository.CreateOrder(userId, dto.ShippingAddress);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            var orderDTO = MapToOrderDTO(result.Data!);

            return Ok(orderDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO orderStatusDTO)
        {
            if (!Enum.TryParse(orderStatusDTO.OrderStatus, out OrderStatus parsedStatus))
            {
                return BadRequest(new { Message = "Invalid order status." });
            }

            var result = await _orderRepository.UpdateOrderStatus(orderId, parsedStatus);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    RepositoryErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    RepositoryErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

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
