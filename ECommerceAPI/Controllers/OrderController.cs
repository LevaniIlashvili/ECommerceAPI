using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PlaceOrder(AddOrderDTO dto)
        {
            var userId = HttpContext.GetAuthenticatedUserId();

            var result = await _orderService.CreateOrder(userId, dto.ShippingAddress);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            };

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO orderStatusDTO)
        {
            if (!Enum.TryParse(orderStatusDTO.OrderStatus, true, out OrderStatus parsedStatus))
            {
                return BadRequest(new { Message = "Invalid order status." });
            }

            var result = await _orderService.UpdateOrderStatus(orderId, parsedStatus);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { Message = result.ErrorMessage }),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersByUserId()
        {
            var userId = HttpContext.GetAuthenticatedUserId();

            var orderDTOs = await _orderService.GetOrdersByUserId(userId);

            return Ok(orderDTOs);
        }
    }
}
