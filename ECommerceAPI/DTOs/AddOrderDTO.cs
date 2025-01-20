using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class AddOrderDTO
    {
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

    }
}
