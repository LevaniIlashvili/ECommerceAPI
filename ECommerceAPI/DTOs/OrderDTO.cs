using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
