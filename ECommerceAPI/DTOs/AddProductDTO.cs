using ECommerceAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.DTOs
{
    public class AddProductDTO
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
