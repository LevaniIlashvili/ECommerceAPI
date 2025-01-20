using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(int userId, string shippingAddress);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<Order> GetOrderById(int id);
        Task UpdateOrderStatus(int orderId, OrderStatus orderStatus);
    }
}
