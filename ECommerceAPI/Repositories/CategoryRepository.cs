using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext _context;

        public CategoryRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<RepositoryResult<Category>> AddCategory(string name)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);

            if (existingCategory != null)
            {
                return RepositoryResult<Category>.Failure("Category already exists", RepositoryErrorType.Conflict);
            }

            var category = new Category { Name = name };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RepositoryResult<Category>.SuccessResult(category);
        }

        public async Task<RepositoryResult<bool>> UpdateCategory(int id, string name)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return RepositoryResult<bool>.Failure("Category doesn't exist", RepositoryErrorType.NotFound);
            }

            category.Name = name;

            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.SuccessResult(true);
        }

        public async Task<RepositoryResult<bool>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return RepositoryResult<bool>.Failure("Category doesn't exist", RepositoryErrorType.NotFound);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RepositoryResult<bool>.SuccessResult(true);
        }
    }
}
