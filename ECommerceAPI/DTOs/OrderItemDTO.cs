using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.DTOs
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }
    }
}
