using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.DTOs
{
    public class AddProductDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = String.Empty;

        [StringLength(500)]
        public string Description { get; set; } = String.Empty;

        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
