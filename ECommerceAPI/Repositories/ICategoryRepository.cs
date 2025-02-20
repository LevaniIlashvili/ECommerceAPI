using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<Category> AddCategory(string name);
        Task<bool> CategoryExists(string name, int? id = null);
        Task<bool> UpdateCategory(int id, string name);
        Task<bool> DeleteCategory(int id);
    }
}
