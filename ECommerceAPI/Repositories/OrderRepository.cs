using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
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

        public async Task<Order> CreateOrder(int userId, string shippingAddress)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                throw new InvalidOperationException("The cart is empty.");
            }

            var Order = new Order
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

            _context.Orders.Add(Order);

            _context.CartItems.RemoveRange(cart.CartItems);

            cart.CartItems.ForEach(ci => ci.Product.Stock -= ci.Quantity);

            await _context.SaveChangesAsync();

            return Order;
        }

        public async Task<Order> GetOrderById(int id)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new ArgumentException("Order doesnt exist");
            }

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                    .ToListAsync();

            if (orders.Count == 0)
            {
                throw new Exception("No orders for user");
            }

            return orders;

        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                throw new Exception("Order doesnt exist");
            }

            order.Status = orderStatus;

            await _context.SaveChangesAsync();
        }
    }
}
