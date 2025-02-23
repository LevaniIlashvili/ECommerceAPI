using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreDbContext _context;

        public OrderRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstAsync(c => c.UserId == order.UserId);


            _context.Orders.Add(order);

            cart.CartItems.ForEach(ci => ci.Product.Stock -= ci.Quantity);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderById(int id)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

            return orders;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _context.Orders.FirstAsync(o => o.Id == orderId);

            order.Status = orderStatus;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasActiveOrders(int productId)
        {
            return await _context.OrderItems
                .AnyAsync(oi => 
                    oi.ProductId == productId &&
                    oi.Order.Status != OrderStatus.Delivered &&
                    oi.Order.Status != OrderStatus.Cancelled);
        }
    }
}
