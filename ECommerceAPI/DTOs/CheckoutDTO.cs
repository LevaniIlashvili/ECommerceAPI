using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class CheckoutDTO
    {
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

    }
}
