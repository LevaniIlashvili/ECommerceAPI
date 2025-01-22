using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs
{
    public class AddCategoryDTO
    {
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;
    }
}
