using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category updatedCategory);
        Task DeleteCategory(int id);
    }
}
