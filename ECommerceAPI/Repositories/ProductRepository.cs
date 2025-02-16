using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
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

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetProducts(string? category = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category != null && p.Category.Name == category);
            }

            var productCount = await query.CountAsync();

            var products = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Include(p => p.Category)
                                .ToListAsync();

            return (products, productCount);
        }

        public async Task<Product?> GetProduct(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1862:Use the 'StringComparison' method overloads to perform case-insensitive string comparisons")]
        public async Task<RepositoryResult<Product>> AddProduct(AddProductDTO productDTO)
        {
            var category = await _context.Categories.FindAsync(productDTO.CategoryId);

            if (category == null)
            {
                return RepositoryResult<Product>.Failure("The specified category doesn't exit", RepositoryErrorType.BadRequest);
            }

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == productDTO.Name.ToLower());

            if (existingProduct != null)
            {
                return RepositoryResult<Product>.Failure("Product with same name already exists", RepositoryErrorType.Conflict);
            }

            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Stock = productDTO.Stock,
                CategoryId = productDTO.CategoryId,
                Category = category
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RepositoryResult<Product>.SuccessResult(product);
        }

        public async Task<RepositoryResult<bool>> UpdateProduct(int id, AddProductDTO updatedProductDTO)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return RepositoryResult<bool>.Failure("Product with specified id not found", RepositoryErrorType.NotFound);
            }

            var category = await _context.Categories.FindAsync(updatedProductDTO.CategoryId);

            if (category == null)
            {
                return RepositoryResult<bool>.Failure("Category doesn't exist", RepositoryErrorType.BadRequest);
            }

            product.Name = updatedProductDTO.Name;
            product.Description = updatedProductDTO.Description;
            product.Price = updatedProductDTO.Price;
            product.Stock = updatedProductDTO.Stock;
            product.CategoryId = updatedProductDTO.CategoryId;

            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.SuccessResult(true);
        }

        public async Task<RepositoryResult<bool>> DeleteProduct(int id)
        {
            var product = await GetProduct(id);

            if (product == null)
            {
                return RepositoryResult<bool>.Failure("Product doesn't exist", RepositoryErrorType.NotFound);
            }

            var hasActiveOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);

            if (hasActiveOrders)
            {
                return RepositoryResult<bool>.Failure("Cannot delete product with active orders", RepositoryErrorType.Conflict);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.SuccessResult(true);
        }



    }
}
