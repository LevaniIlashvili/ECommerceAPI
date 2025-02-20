using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Result<Category>> AddCategoryAsync(AddCategoryDTO categoryDTO);
        Task<Result<bool>> UpdateCategoryAsync(int id, AddCategoryDTO categoryDTO);
        Task<Result<bool>> DeleteCategoryAsync(int id);
    }
}
