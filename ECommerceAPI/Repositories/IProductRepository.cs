using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProduct(int id);
        Task<IEnumerable<Product>> GetProducts();
        Task AddProduct(Product product);
        Task UpdateProduct(Product updatedProduct);
        Task DeleteProduct(int id);
    }
}
