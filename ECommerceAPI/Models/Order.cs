namespace ECommerceAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string ShippingAddress { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public User User { get; set; }
    }
}
