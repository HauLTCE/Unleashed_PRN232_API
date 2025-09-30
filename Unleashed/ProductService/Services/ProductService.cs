using AutoMapper;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IVariationRepository _variationRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository,
            IVariationRepository variationRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _variationRepository = variationRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDetailDTO>> GetPagedProductsAsync(PaginationParams pagination)
        {
            var pagedProducts = await _productRepository.GetPagedAsync(pagination);

            var productDtos = new List<ProductDetailDTO>();

            foreach (var product in pagedProducts.Items)
            {
                var productDto = _mapper.Map<ProductDetailDTO>(product);
                var categories = await _productCategoryRepository.GetCategoriesByProductIdAsync(product.ProductId);
                productDto.Categories = _mapper.Map<List<CategoryDetailDTO>>(categories);
                productDtos.Add(productDto);
            }

            return new PagedResult<ProductDetailDTO>
            {
                Items = productDtos,
                TotalCount = pagedProducts.TotalCount,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize
            };
        }

       
        public async Task<ProductDetailDTO?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            var productDto = _mapper.Map<ProductDetailDTO>(product);
            var categories = await _productCategoryRepository.GetCategoriesByProductIdAsync(id);
            productDto.Categories = _mapper.Map<List<CategoryDetailDTO>>(categories);

            return productDto;
        }

        public async Task<ProductDetailDTO> CreateProductAsync(CreateProductDTO createProductDto)
        {
            // Validation
            if (createProductDto.BrandId.HasValue && !await _brandRepository.ExistsAsync(createProductDto.BrandId.Value))
                throw new ArgumentException("Brand does not exist");

            var product = _mapper.Map<Product>(createProductDto);
            product.ProductId = Guid.NewGuid();
            product.ProductCreatedAt = DateTimeOffset.UtcNow;

            var createdProduct = await _productRepository.CreateAsync(product);

            // Add categories
            if (createProductDto.CategoryIds != null && createProductDto.CategoryIds.Any())
            {
                await _productCategoryRepository.AddCategoriesToProductAsync(createdProduct.ProductId, createProductDto.CategoryIds);
            }

            return await GetProductByIdAsync(createdProduct.ProductId);
        }

        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return null;

            _mapper.Map(updateProductDto, existingProduct);
            existingProduct.ProductUpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.UpdateAsync(existingProduct);
            return await GetProductByIdAsync(id);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productRepository.DeleteAsync(id);
        }
    }
}
