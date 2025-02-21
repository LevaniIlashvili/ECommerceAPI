using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public interface IProductService
    {
        Task<Result<(IEnumerable<ProductDTO>, int)>> GetAllProductsAsync(string? category = null, int pageNumber = 1, int pageSize = 10);
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<Result<ProductDTO>> AddProductAsync(AddProductDTO productDTO);
        Task<Result<bool>> UpdateProductAsync(int id, AddProductDTO productDTO);
        Task<Result<bool>> DeleteProductAsync(int id);
    }
}
