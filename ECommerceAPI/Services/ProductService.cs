using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<Result<(IEnumerable<ProductDTO>, int)>> GetAllProductsAsync(string? category = null, int pageNumber = 1, int pageSize = 10)
        {
            if (!string.IsNullOrWhiteSpace(category) && !await _categoryRepository.CategoryExists(category))
            {
                return Result<(IEnumerable<ProductDTO> Products, int TotalCount)>.Failure("specified category doesn't exist", ErrorType.NotFound);
            }

            var (products, productCount) = await _productRepository.GetProducts(category, pageNumber, pageSize);

            var productDTOs = _mapper.Map<List<ProductDTO>>(products);

            return Result<(IEnumerable<ProductDTO>, int)>.SuccessResult((productDTOs, productCount));
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProduct(id);

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<Result<ProductDTO>> AddProductAsync(AddProductDTO addProductDTO)
        {
            var category = await _categoryRepository.GetCategory(addProductDTO.CategoryId);

            if (category == null)
            {
                return Result<ProductDTO>.Failure("The specified category doesn't exit", ErrorType.BadRequest);
            }

            if (await _productRepository.ProductExists(addProductDTO.Name))
            {
                return Result<ProductDTO>.Failure("Product with same name already exists", ErrorType.Conflict);
            }

            var product = await _productRepository.AddProduct(addProductDTO);

            var productDTO = _mapper.Map<ProductDTO>(product);

            productDTO.Category = category;

            return Result<ProductDTO>.SuccessResult(productDTO);
        }

        public async Task<Result<bool>> UpdateProductAsync(int id, AddProductDTO updateProductDTO)
        {
            var product = await _productRepository.GetProduct(id);

            if (product == null)
            {
                return Result<bool>.Failure("Product with specified id not found", ErrorType.NotFound);
            }

            var category = await _categoryRepository.GetCategory(updateProductDTO.CategoryId);

            if (category == null)
            {
                return Result<bool>.Failure("Category doesn't exist", ErrorType.BadRequest);
            }

            await _productRepository.UpdateProduct(id, updateProductDTO);

            return Result<bool>.SuccessResult(true);
        }

        public async Task<Result<bool>> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProduct(id);

            if (product == null)
            {
                return Result<bool>.Failure("Product with specified id not found", ErrorType.NotFound);
            }

            var hasActiveOrders = await _orderRepository.HasActiveOrders(id);

            if (hasActiveOrders)
            {
                return Result<bool>.Failure("Cannot delete product with active orders", ErrorType.Conflict);
            }

            await _productRepository.DeleteProduct(id);

            return Result<bool>.SuccessResult(true);
        }
    }
}
