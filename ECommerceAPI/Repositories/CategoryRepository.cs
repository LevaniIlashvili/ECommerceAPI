using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories
{
    public class CategoryRepository(StoreDbContext context) : ICategoryRepository
    {
        private readonly StoreDbContext _context = context;

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetCategory(int id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> AddCategory(string name)
        {
            var category = new Category { Name = name };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> CategoryExists(string name, int? id = null)
        {
             return await _context.Categories.AnyAsync(c => c.Name == name && (!id.HasValue || c.Id != id.Value));
        }

        public async Task<bool> UpdateCategory(int id, string name)
        {
            var category = await _context.Categories.FindAsync(id);
            category!.Name = name;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category!);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
