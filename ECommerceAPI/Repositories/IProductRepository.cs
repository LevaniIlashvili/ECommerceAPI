using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProduct(int id);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetProducts(string? category = null, int pageNumber = 1, int pageSize = 10);
        Task<Product> AddProduct(AddProductDTO product);
        Task<bool> ProductExists(string name);
        Task UpdateProduct(int id, AddProductDTO updatedProduct);
        Task DeleteProduct(int id);
    }
}
