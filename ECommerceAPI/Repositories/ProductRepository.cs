using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext _context;

        public ProductRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetProducts(CancellationToken cancellationToken, string? category = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            var productCount = await query.CountAsync(cancellationToken);

            var products = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Include(p => p.Category)
                                .ToListAsync(cancellationToken);

            return (products, productCount);
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons")]
        public async Task<Product> AddProduct(AddProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = productDTO.Stock,
                CategoryId = productDTO.CategoryId,
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> ProductExists(string name)
        {
            return await _context.Products.AnyAsync(p => p.Name == name);
        }

        public async Task UpdateProduct(int id, AddProductDTO updateProductDTO)
        {
            var product = await _context.Products.FindAsync(id);

            product!.Name = updateProductDTO.Name;
            product.Description = updateProductDTO.Description;
            product.Price = updateProductDTO.Price;
            product.Stock = updateProductDTO.Stock;
            product.CategoryId = updateProductDTO.CategoryId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var product = await GetProduct(id);

            _context.Products.Remove(product!);
            await _context.SaveChangesAsync();
        }
    }
}
