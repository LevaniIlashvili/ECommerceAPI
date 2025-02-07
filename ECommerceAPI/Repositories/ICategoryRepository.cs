using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategory(int id);
        Task<RepositoryResult<Category>> AddCategory(string name);
        Task<RepositoryResult<bool>> UpdateCategory(int id, string name);
        Task<RepositoryResult<bool>> DeleteCategory(int id);
    }
}
