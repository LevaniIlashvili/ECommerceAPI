using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class AddCartItemDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
