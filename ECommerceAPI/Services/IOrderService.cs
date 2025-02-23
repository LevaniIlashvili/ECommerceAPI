using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public interface IOrderService
    {
        Task<Result<OrderDTO>> CreateOrder(int userId, string shippingAddress);
        Task<Result<bool>> UpdateOrderStatus(int orderId, OrderStatus orderStatus);
        Task<IEnumerable<OrderDTO>> GetOrdersByUserId(int userId);
    }
}
