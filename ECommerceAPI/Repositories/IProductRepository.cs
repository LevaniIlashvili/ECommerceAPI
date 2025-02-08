using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProduct(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task<RepositoryResult<Product>> AddProduct(AddProductDTO product);
        Task<RepositoryResult<bool>> UpdateProduct(int id, AddProductDTO updatedProduct);
        Task<RepositoryResult<bool>> DeleteProduct(int id);
    }
}
