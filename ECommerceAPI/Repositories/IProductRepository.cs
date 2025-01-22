using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProduct(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> AddProduct(AddProductDTO product);
        Task UpdateProduct(int id, AddProductDTO updatedProduct);
        Task DeleteProduct(int id);
    }
}
