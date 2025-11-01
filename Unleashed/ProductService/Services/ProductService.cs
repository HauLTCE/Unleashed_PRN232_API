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
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IVariationRepository _variationRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly IColorRepository _colorRepository;
        private readonly ILogger<ProductServiceImpl> _logger;

        public ProductServiceImpl(
      IMapper mapper,
      IProductRepository productRepository,
      IProductCategoryRepository productCategoryRepository,
      IVariationRepository variationRepository,
      IBrandRepository brandRepository,
      ICategoryRepository categoryRepository,
      ISizeRepository sizeRepository,
      IColorRepository colorRepository,
      ILogger<ProductServiceImpl> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _variationRepository = variationRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _sizeRepository = sizeRepository;
            _colorRepository = colorRepository;
            _logger = logger;
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

        public async Task<ProductDetailDTO> CreateProductAsync(CreateProductDTO dto)
        {
            // Check trùng tên
            if (await _productRepository.GetByProductNameAsync(dto.ProductName) != null)
                throw new ArgumentException($"Product with name '{dto.ProductName}' already exists.");

            // Validate Brand và Category
            if (dto.BrandId.HasValue && !await _brandRepository.ExistsAsync(dto.BrandId.Value))
                throw new ArgumentException("Brand does not exist");

            if (dto.CategoryIds?.Any() == true)
            {
                foreach (var cid in dto.CategoryIds.Distinct())
                    if (!await _categoryRepository.ExistsAsync(cid))
                        throw new ArgumentException($"Category with ID {cid} does not exist");
            }

            // Tạo entity Product (KHÔNG map variations)
            var product = _mapper.Map<Product>(dto);
            product.ProductId = Guid.NewGuid();
            product.Variations = null; // ✅ bỏ mapping variations ở đây
            product.ProductCreatedAt = DateTimeOffset.UtcNow;
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.CreateAsync(product);
            var productId = product.ProductId;

            // Gắn categories
            if (dto.CategoryIds?.Any() == true)
                await _productCategoryRepository.AddCategoriesToProductAsync(productId, dto.CategoryIds);

            // Gắn variations
            if (dto.Variations?.Any() == true)
            {
                foreach (var vdto in dto.Variations)
                {
                    if (!vdto.SizeId.HasValue || !vdto.ColorId.HasValue)
                        continue;

                    var size = await _sizeRepository.GetByIdAsync(vdto.SizeId.Value);
                    var color = await _colorRepository.GetByIdAsync(vdto.ColorId.Value);
                    if (size == null || color == null)
                        continue;

                    var variation = _mapper.Map<Variation>(vdto);
                    variation.ProductId = productId;
                    await _variationRepository.CreateAsync(variation);
                }
            }

            await _productRepository.SaveChangesAsync();

            return await GetProductByIdAsync(productId);
        }





        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            // Kiểm tra tên sản phẩm trùng
            var existingProduct = await _productRepository.GetByProductNameAsync(dto.ProductName);
            if (existingProduct != null && existingProduct.ProductId != id)
                throw new ArgumentException($"Product with name {dto.ProductName} already exists.");

            // Cập nhật thuộc tính cơ bản
            _mapper.Map(dto, product);
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            // --- Update Categories ---
            if (dto.CategoryIds != null)
            {
                var incoming = dto.CategoryIds.Distinct().ToList();
                var current = (await _productCategoryRepository.GetCategoriesByProductIdAsync(id))
                              .Select(c => c.CategoryId).ToList();

                var toAdd = incoming.Except(current).ToList();
                var toRemove = current.Except(incoming).ToList();

                if (toRemove.Any())
                    await _productCategoryRepository.RemoveCategoriesFromProductAsync(id, toRemove);
                if (toAdd.Any())
                    await _productCategoryRepository.AddCategoriesToProductAsync(id, toAdd);
            }

            // --- Update Variations ---
            if (dto.Variations != null)
            {
                var existingVariations = await _variationRepository.GetByProductIdAsync(id);
                var existingById = existingVariations.ToDictionary(v => v.VariationId);
                var keepIds = new HashSet<int>();

                foreach (var vdto in dto.Variations)
                {
                    // Kiểm tra Size, Color hợp lệ
                    if (vdto.SizeId.HasValue && await _sizeRepository.GetByIdAsync(vdto.SizeId.Value) == null)
                        throw new ArgumentException($"Invalid SizeId={vdto.SizeId.Value}");
                    if (vdto.ColorId.HasValue && await _colorRepository.GetByIdAsync(vdto.ColorId.Value) == null)
                        throw new ArgumentException($"Invalid ColorId={vdto.ColorId.Value}");

                    if (vdto.VariationId > 0 && existingById.TryGetValue(vdto.VariationId, out var existingVar))
                    {
                        // ✅ Cập nhật variation cũ
                        _mapper.Map(vdto, existingVar);
                        await _variationRepository.UpdateAsync(existingVar);
                        keepIds.Add(existingVar.VariationId);
                    }
                    else
                    {
                        // ✅ Thêm mới variation
                        if (!vdto.SizeId.HasValue || !vdto.ColorId.HasValue || !vdto.VariationPrice.HasValue)
                            throw new ArgumentException("New variation requires SizeId, ColorId, and VariationPrice");

                        var newVar = _mapper.Map<Variation>(vdto);
                        newVar.ProductId = id;

                        var createdVar = await _variationRepository.CreateAsync(newVar);
                        keepIds.Add(createdVar.VariationId);
                    }
                }

                // ✅ Xóa variation không còn trong danh sách mới
                var toDelete = existingVariations
                    .Where(v => !keepIds.Contains(v.VariationId))
                    .Select(v => v.VariationId)
                    .ToList();

                foreach (var vid in toDelete)
                    await _variationRepository.DeleteAsync(vid);
            }

            // --- Lưu thay đổi ---
            await _productRepository.SaveChangesAsync();

            // --- Trả về kết quả đầy đủ ---
            return await GetProductByIdAsync(id);
        }



        public async Task<bool> DeleteVariationAsync(int variationId)
{
    return await _variationRepository.DeleteAsync(variationId);
}

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            // 0) Check tồn tại
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            try
            {
                // 1) Xoá variations
                await _variationRepository.DeleteByProductIdAsync(id);

                // 2) Xoá tất cả liên kết categories (Raw SQL)
                await _productCategoryRepository.RemoveAllByProductAsync(id);

                // 3) Xoá product
                var ok = await _productRepository.DeleteAsync(id);
                return ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteProduct failed. ProductId={ProductId}", id);
                throw; // controller sẽ trả 500
            }
        }

        public async Task<ProductForWishlistDTO?> GetProductInfoForWishlistAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdWithFirstVariationAsync(productId);

            if (product == null)
            {
                return null;
            }

            var dto = _mapper.Map<ProductForWishlistDTO>(product);
            dto.ProductImage = product.Variations.FirstOrDefault()?.VariationImage;

            return dto;
        }


        public async Task<IEnumerable<ProductSummaryDTO>> GetProductSummariesByIdsAsync(IEnumerable<Guid> ids)
        {
            var products = await _productRepository.GetByIdsAsync(ids);

            return products.Select(p => new ProductSummaryDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                // Take the first variation's image as the product image
                ProductImageUrl = p.Variations.FirstOrDefault()?.VariationImage
            });
        }

    }
}