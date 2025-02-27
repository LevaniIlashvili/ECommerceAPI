using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CacheService _cacheService;

        public CategoryService(ICategoryRepository categoryRepository, CacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
            
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var cacheKey = "categories";
            var cachedData = await _cacheService.GetFromCacheAsync<IEnumerable<Category>>(cacheKey);
            if (cachedData != null) return cachedData;

            var categories = await _categoryRepository.GetCategories();

            await _cacheService.SetCacheAsync(cacheKey, categories);

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            var cachedData = await _cacheService.GetFromCacheAsync<Category>($"category:{id}");
            if (cachedData != null) return cachedData;

            var category = await _categoryRepository.GetCategory(id);

            await _cacheService.SetCacheAsync($"category:{id}", category);

            return category;
        }

        public async Task<Result<Category>> AddCategoryAsync(AddCategoryDTO categoryDTO)
        {
            if (await _categoryRepository.CategoryExists(categoryDTO.Name))
            {
                return Result<Category>.Failure("Category with that name already exists", ErrorType.Conflict);
            }

            var category = await _categoryRepository.AddCategory(categoryDTO.Name);

            await _cacheService.RemoveCacheAsync("categories");

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

            await _cacheService.RemoveCacheAsync("categories");
            await _cacheService.RemoveCacheAsync($"category:{id}");

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

            await _cacheService.RemoveCacheAsync("categories");
            await _cacheService.RemoveCacheAsync($"category:{id}");

            return Result<bool>.SuccessResult(deleted);
        }
    }
}
