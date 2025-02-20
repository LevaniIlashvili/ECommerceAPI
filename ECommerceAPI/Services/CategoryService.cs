using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetCategories();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategory(id);
        }

        public async Task<Result<Category>> AddCategoryAsync(AddCategoryDTO categoryDTO)
        {
            if (await _categoryRepository.CategoryExists(categoryDTO.Name))
            {
                return Result<Category>.Failure("Category with that name already exists", ErrorType.Conflict);
            }

            var category = await _categoryRepository.AddCategory(categoryDTO.Name);

            return Result<Category>.SuccessResult(category);
        }

        public async Task<Result<bool>> UpdateCategoryAsync(int id, AddCategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetCategory(id);

            if (category == null)
            {
                return Result<bool>.Failure("Category doesn't exist", ErrorType.NotFound);
            }

            if (await _categoryRepository.CategoryExists(categoryDTO.Name, id))
            {
                return Result<bool>.Failure("Category with that name already exists", ErrorType.Conflict);
            }

            var updated = await _categoryRepository.UpdateCategory(id, categoryDTO.Name);
            return Result<bool>.SuccessResult(updated);
        }

        public async Task<Result<bool>> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategory(id);

            if (category == null)
            {
                return Result<bool>.Failure("Category doesn't exist", ErrorType.NotFound);
            }

            var deleted = await _categoryRepository.DeleteCategory(id);
            return Result<bool>.SuccessResult(deleted);
        }
    }
}
