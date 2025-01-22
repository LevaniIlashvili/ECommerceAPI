using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<Category> AddCategory(string name);
        Task UpdateCategory(int id, string name);
        Task DeleteCategory(int id);
    }
}
