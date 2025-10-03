using AutoMapper;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.DTOs.VariationDTOs;
using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IVariationRepository _variationRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductServiceImpl(
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

            // Validate categories
            if (createProductDto.CategoryIds != null && createProductDto.CategoryIds.Any())
            {
                foreach (var categoryId in createProductDto.CategoryIds)
                {
                    if (!await _categoryRepository.ExistsAsync(categoryId))
                        throw new ArgumentException($"Category with ID {categoryId} does not exist");
                }
            }

            var product = _mapper.Map<Product>(createProductDto);
            product.ProductId = Guid.NewGuid();
            product.ProductCreatedAt = DateTimeOffset.UtcNow;

            var createdProduct = await _productRepository.CreateAsync(product);

            // Add categories
            if (createProductDto.CategoryIds != null && createProductDto.CategoryIds.Any())
            {
                await _productCategoryRepository.AddCategoriesToProductAsync(createdProduct.ProductId, createProductDto.CategoryIds);
            }

            // Add variations
            if (createProductDto.Variations != null && createProductDto.Variations.Any())
            {
                foreach (var variationDto in createProductDto.Variations)
                {
                    var variation = _mapper.Map<Variation>(variationDto);
                    variation.ProductId = createdProduct.ProductId;
                    await _variationRepository.CreateAsync(variation);
                }
            }

            return await GetProductByIdAsync(createdProduct.ProductId);
        }

        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null) return null;

            // Update product properties
            existingProduct.ProductName = updateProductDto.ProductName ?? existingProduct.ProductName;
            existingProduct.ProductCode = updateProductDto.ProductCode ?? existingProduct.ProductCode;
            existingProduct.ProductDescription = updateProductDto.ProductDescription ?? existingProduct.ProductDescription;
            existingProduct.BrandId = updateProductDto.BrandId ?? existingProduct.BrandId;
            existingProduct.ProductStatusId = updateProductDto.ProductStatusId ?? existingProduct.ProductStatusId;
            existingProduct.ProductUpdatedAt = DateTimeOffset.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Update categories if provided
            if (updateProductDto.CategoryIds != null)
            {
                // Get current categories
                var currentCategories = await _productCategoryRepository.GetCategoriesByProductIdAsync(id);
                var currentCategoryIds = currentCategories.Select(c => c.CategoryId).ToList();

                // Remove all current categories
                if (currentCategoryIds.Any())
                {
                    await _productCategoryRepository.RemoveCategoriesFromProductAsync(id, currentCategoryIds);
                }

                // Add new categories
                await _productCategoryRepository.AddCategoriesToProductAsync(id, updateProductDto.CategoryIds);
            }

            return await GetProductByIdAsync(id);
        }

        public async Task<ProductDetailDTO?> UpdateProductVariationsAsync(Guid productId, List<UpdateVariationDTO> updateVariationDtos)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productId);
            if (existingProduct == null) return null;

            // Get existing variations for this product
            var existingVariations = await _variationRepository.GetByProductIdAsync(productId);

            // Update or create variations
            foreach (var variationDto in updateVariationDtos)
            {
                if (variationDto.VariationId > 0) // Sửa từ HasValue sang > 0
                {
                    // Update existing variation
                    var existingVariation = existingVariations.FirstOrDefault(v => v.VariationId == variationDto.VariationId);
                    if (existingVariation != null)
                    {
                        _mapper.Map(variationDto, existingVariation);
                        await _variationRepository.UpdateAsync(existingVariation);
                    }
                }
                else
                {
                    // Create new variation
                    var newVariation = _mapper.Map<Variation>(variationDto);
                    newVariation.ProductId = productId;
                    await _variationRepository.CreateAsync(newVariation);
                }
            }

            // Return updated product
            return await GetProductByIdAsync(productId);
        }

        public async Task<bool> DeleteVariationAsync(int variationId)
{
    return await _variationRepository.DeleteAsync(variationId);
}

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productRepository.DeleteAsync(id);
        }
    }
}