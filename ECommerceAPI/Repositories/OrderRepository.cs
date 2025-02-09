using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
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

        public async Task<RepositoryResult<Order>> CreateOrder(int userId, string shippingAddress)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
            {
                return RepositoryResult<Order>.Failure("Cart is empty", RepositoryErrorType.BadRequest);
            }

            if (cart.CartItems.Any(ci => ci.Quantity > ci.Product.Stock))
            {
                return RepositoryResult<Order>.Failure("One or more items in your cart are out of stock.", RepositoryErrorType.BadRequest);
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

            cart.CartItems.ForEach(ci => ci.Product.Stock -= ci.Quantity);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return RepositoryResult<Order>.SuccessResult(Order);
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

            return orders;
        }

        public async Task<RepositoryResult<bool>> UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return RepositoryResult<bool>.Failure("Order doesn't exist", RepositoryErrorType.NotFound);
            }

            if (!Enum.IsDefined(typeof(OrderStatus), orderStatus))
            {
                return RepositoryResult<bool>.Failure("Invalid order status.", RepositoryErrorType.BadRequest);
            }

            order.Status = orderStatus;

            await _context.SaveChangesAsync();

            return RepositoryResult<bool>.SuccessResult(true);
        }
    }
}
