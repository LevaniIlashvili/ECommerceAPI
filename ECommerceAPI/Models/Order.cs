using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingAddress { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public User User { get; set; }
    }
}
