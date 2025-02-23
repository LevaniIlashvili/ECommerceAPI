using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<Order?> GetOrderById(int id);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatus orderStatus);
        Task<bool> HasActiveOrders(int productId);
    }
}
