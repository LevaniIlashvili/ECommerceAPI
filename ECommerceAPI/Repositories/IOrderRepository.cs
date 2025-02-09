using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<RepositoryResult<Order>> CreateOrder(int userId, string shippingAddress);
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
        Task<Order> GetOrderById(int id);
        Task<RepositoryResult<bool>> UpdateOrderStatus(int orderId, OrderStatus orderStatus);
    }
}
