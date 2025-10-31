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
            // 0) Validate giống code cũ
            if (dto.BrandId.HasValue && !await _brandRepository.ExistsAsync(dto.BrandId.Value))
                throw new ArgumentException("Brand does not exist");

            if (dto.CategoryIds?.Any() == true)
            {
                foreach (var cid in dto.CategoryIds.Distinct())
                    if (!await _categoryRepository.ExistsAsync(cid))
                        throw new ArgumentException($"Category with ID {cid} does not exist");
            }

            // 2) Tạo product entity
            var product = _mapper.Map<Product>(dto);
            product.ProductId = Guid.NewGuid();
            product.ProductCreatedAt = DateTimeOffset.UtcNow;
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            // 3) Bắt đầu giao dịch thủ công
            await _productRepository.BeginTransactionAsync(); // Không cần gán vào biến

            try
            {
                // 4) Tạo sản phẩm vào cơ sở dữ liệu
                await _productRepository.CreateAsync(product);

                var productId = product.ProductId;

                // 5) Thêm categories cho sản phẩm
                if (dto.CategoryIds?.Any() == true)
                {
                    await _productCategoryRepository.AddCategoriesToProductAsync(productId, dto.CategoryIds);
                }

                // 6) Kiểm tra và thêm variations cho sản phẩm
                if (dto.Variations?.Any() == true)
                {
                    var existingVariations = await _variationRepository.GetByProductIdAsync(productId);

                    foreach (var vdto in dto.Variations)
                    {
                        if (!vdto.SizeId.HasValue || !vdto.ColorId.HasValue)
                            continue;

                        var size = await _sizeRepository.GetByIdAsync(vdto.SizeId.Value);
                        var color = await _colorRepository.GetByIdAsync(vdto.ColorId.Value);

                        if (size == null || color == null)
                            continue;

                        // Kiểm tra sự trùng lặp của variation trước khi thêm
                        var existingVariation = existingVariations.FirstOrDefault(v =>
                            v.SizeId == vdto.SizeId && v.ColorId == vdto.ColorId && v.VariationPrice == vdto.VariationPrice);

                        if (existingVariation == null)
                        {
                            var variation = _mapper.Map<Variation>(vdto);
                            variation.ProductId = productId; // gắn ProductId vào Variation
                            await _variationRepository.CreateAsync(variation);
                        }
                    }
                }

                // 7) Lưu tất cả thay đổi vào cơ sở dữ liệu trong giao dịch
                await _productRepository.SaveChangesAsync();  // Lưu tất cả thay đổi

                // 8) Commit giao dịch
                await _productRepository.CommitTransactionAsync(); // Đảm bảo commit giao dịch nếu có phương thức này

                // 9) Trả về DTO sản phẩm đã tạo
                return await GetProductByIdAsync(productId);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, hủy bỏ giao dịch
                await _productRepository.RollbackTransactionAsync(); // Hủy bỏ giao dịch nếu có lỗi
                _logger.LogError(ex, "CreateProduct failed, rolling back. ProductId={ProductId}", product.ProductId);
                throw;
            }
        }




        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            // Kiểm tra tên sản phẩm có trùng với sản phẩm khác không (ngoại trừ sản phẩm hiện tại)
            var existingProduct = await _productRepository.GetByProductNameAsync(dto.ProductName);
            if (existingProduct != null && existingProduct.ProductId != id)
            {
                throw new ArgumentException($"Product with name {dto.ProductName} already exists.");
            }

            // 1) Cập nhật các thuộc tính scalar (ví dụ: tên sản phẩm, mã sản phẩm, mô tả, v.v.)
            _mapper.Map(dto, product);
            product.ProductUpdatedAt = DateTimeOffset.UtcNow; // Cập nhật thời gian sửa đổi

            // 2) Cập nhật danh mục cho sản phẩm
            if (dto.CategoryIds != null)
            {
                var incomingCategories = dto.CategoryIds.Distinct().ToList();
                var currentCategories = (await _productCategoryRepository.GetCategoriesByProductIdAsync(id))
                                        .Select(c => c.CategoryId).ToList();

                // Các category cần xóa (không còn trong danh sách cập nhật)
                var categoriesToRemove = currentCategories.Except(incomingCategories).ToList();
                // Các category cần thêm (có trong danh sách cập nhật mà không có trong hiện tại)
                var categoriesToAdd = incomingCategories.Except(currentCategories).ToList();

                if (categoriesToRemove.Any())
                    await _productCategoryRepository.RemoveCategoriesFromProductAsync(id, categoriesToRemove);
                if (categoriesToAdd.Any())
                    await _productCategoryRepository.AddCategoriesToProductAsync(id, categoriesToAdd);
            }

            // 3) Cập nhật hoặc thêm mới variations
            if (dto.Variations != null)
            {
                var existingVariations = await _variationRepository.GetByProductIdAsync(id);
                var variationsById = existingVariations.ToDictionary(v => v.VariationId);
                var variationsToKeep = new HashSet<int>();

                foreach (var variationDto in dto.Variations)
                {
                    // Kiểm tra xem Size và Color có hợp lệ không
                    if (variationDto.SizeId.HasValue && await _sizeRepository.GetByIdAsync(variationDto.SizeId.Value) == null)
                        throw new ArgumentException($"Invalid SizeId={variationDto.SizeId.Value}");
                    if (variationDto.ColorId.HasValue && await _colorRepository.GetByIdAsync(variationDto.ColorId.Value) == null)
                        throw new ArgumentException($"Invalid ColorId={variationDto.ColorId.Value}");

                    if (variationDto.VariationId > 0 && variationsById.TryGetValue(variationDto.VariationId, out var existingVariation))
                    {
                        // Cập nhật các variation đã tồn tại
                        _mapper.Map(variationDto, existingVariation);
                        await _variationRepository.UpdateAsync(existingVariation);
                        variationsToKeep.Add(existingVariation.VariationId);
                    }
                    else
                    {
                        // Thêm mới variation nếu không có
                        if (!variationDto.SizeId.HasValue || !variationDto.ColorId.HasValue || !variationDto.VariationPrice.HasValue)
                            throw new ArgumentException("New variation requires SizeId, ColorId, and VariationPrice");

                        var newVariation = _mapper.Map<Variation>(variationDto);
                        newVariation.ProductId = id; // Gán ProductId vào Variation mới

                        var createdVariation = await _variationRepository.CreateAsync(newVariation);
                        variationsToKeep.Add(createdVariation.VariationId);
                    }
                }

                // Xóa các variations không còn trong danh sách (các variation không được giữ lại)
                var variationsToDelete = existingVariations.Where(v => !variationsToKeep.Contains(v.VariationId))
                                                           .Select(v => v.VariationId)
                                                           .ToList();
                foreach (var variationId in variationsToDelete)
                {
                    await _variationRepository.DeleteAsync(variationId);
                }
            }

            // 4) Lưu thay đổi vào cơ sở dữ liệu
            await _productRepository.SaveChangesAsync(); // Lưu tất cả thay đổi trong giao dịch

            // 5) Trả về DTO đầy đủ của sản phẩm sau khi cập nhật
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