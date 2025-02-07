using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class AddCategoryDTO
    {
        [MinLength(2, ErrorMessage = "Category name must be at least 2 characters long")]
        public string Name { get; set; } = string.Empty;
    }
}
