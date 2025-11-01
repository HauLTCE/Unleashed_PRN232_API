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

        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO dto)
        {
            // 1. Lấy product hiện tại
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                // Không tìm thấy sản phẩm -> controller trả 404
                return null;
            }

            // 2. Kiểm tra trùng tên sản phẩm nếu có đổi tên
            if (!string.IsNullOrWhiteSpace(dto.ProductName))
            {
                var existingByName = await _productRepository.GetByProductNameAsync(dto.ProductName);

                // Nếu tìm thấy 1 sản phẩm khác có cùng tên => lỗi
                if (existingByName != null && existingByName.ProductId != id)
                {
                    throw new ArgumentException(
                        $"Product with name '{dto.ProductName}' already exists.");
                }
            }

            // 3. Validate Brand nếu có gửi BrandId
            if (dto.BrandId.HasValue)
            {
                var brandExists = await _brandRepository.ExistsAsync(dto.BrandId.Value);
                if (!brandExists)
                {
                    throw new ArgumentException("Brand does not exist");
                }
            }

            // 4. Validate CategoryIds nếu có gửi CategoryIds
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                foreach (var cid in dto.CategoryIds.Distinct())
                {
                    if (!await _categoryRepository.ExistsAsync(cid))
                        throw new ArgumentException($"Category with ID {cid} does not exist");
                }
            }

            // 5. Cập nhật thủ công từng field cho product
            //    -> không dùng AutoMapper.Map(dto, product) để tránh conflict EF tracking
            if (dto.ProductName != null)
                product.ProductName = dto.ProductName;

            if (dto.ProductCode != null)
                product.ProductCode = dto.ProductCode;

            if (dto.ProductDescription != null)
                product.ProductDescription = dto.ProductDescription;

            if (dto.BrandId.HasValue)
                product.BrandId = dto.BrandId.Value;

            if (dto.ProductStatusId.HasValue)
                product.ProductStatusId = dto.ProductStatusId.Value;

            // timestamp
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            // 6. Lưu thay đổi thông tin sản phẩm chính
            await _productRepository.UpdateAsync(product);


            // 7. Đồng bộ Category nếu DTO có gửi CategoryIds
            //
            // - dto.CategoryIds == null  -> không đụng tới categories hiện tại
            // - dto.CategoryIds != null  -> xoá hết cũ, add lại đúng danh sách dto gửi
            if (dto.CategoryIds != null)
            {
                // Xoá hết category cũ
                await _productCategoryRepository.RemoveAllByProductAsync(id);

                // Gán lại category mới (nếu có)
                if (dto.CategoryIds.Any())
                {
                    await _productCategoryRepository.AddCategoriesToProductAsync(id, dto.CategoryIds);
                }
            }


            // 8. Đồng bộ Variations nếu DTO có gửi Variations
            //
            // QUY ƯỚC THEO DTO CỦA SẾP:
            //  - VariationId > 0  => Variation cũ, UPDATE
            //  - VariationId <= 0 => Variation mới, CREATE
            //
            // Sau đó xoá những variation cũ nào không còn xuất hiện trong dto.
            if (dto.Variations != null)
            {
                // 8.1. Lấy danh sách variation hiện có TRƯỚC khi chỉnh
                var existingVarsBefore = await _variationRepository.GetByProductIdAsync(id);
                var existingVarIdsBefore = existingVarsBefore
                    .Select(v => v.VariationId)
                    .ToList();

                // 8.2. Lặp qua từng variation DTO để update hoặc create
                foreach (var vDto in dto.Variations)
                {
                    // Validate Size / Color nếu có
                    if (vDto.SizeId.HasValue && !await _sizeRepository.ExistsAsync(vDto.SizeId.Value))
                        throw new ArgumentException($"Size with ID {vDto.SizeId.Value} does not exist");

                    if (vDto.ColorId.HasValue && !await _colorRepository.ExistsAsync(vDto.ColorId.Value))
                        throw new ArgumentException($"Color with ID {vDto.ColorId.Value} does not exist");

                    // CASE 1: Variation cũ (VariationId > 0) -> UPDATE
                    if (vDto.VariationId > 0)
                    {
                        var existingVar = await _variationRepository.GetByIdAsync(vDto.VariationId);
                        if (existingVar == null)
                        {
                            throw new ArgumentException(
                                $"Variation with ID {vDto.VariationId} does not exist");
                        }

                        // Cập nhật từng trường nếu có gửi
                        if (vDto.SizeId.HasValue)
                            existingVar.SizeId = vDto.SizeId.Value;

                        if (vDto.ColorId.HasValue)
                            existingVar.ColorId = vDto.ColorId.Value;

                        if (vDto.VariationImage != null)
                            existingVar.VariationImage = vDto.VariationImage;

                        if (vDto.VariationPrice.HasValue)
                            existingVar.VariationPrice = vDto.VariationPrice.Value;

                        // đảm bảo không lạc sang product khác
                        existingVar.ProductId = id;

                        await _variationRepository.UpdateAsync(existingVar);
                    }
                    // CASE 2: Variation mới (VariationId <= 0) -> CREATE
                    else
                    {
                        var newVar = new Variation
                        {
                            ProductId = id,
                            SizeId = vDto.SizeId ?? null,
                            ColorId = vDto.ColorId ?? null,
                            VariationImage = vDto.VariationImage,
                            VariationPrice = vDto.VariationPrice ?? 0
                        };

                        await _variationRepository.CreateAsync(newVar);
                    }
                }

                // 8.3. Xoá những variation cũ nào KHÔNG còn trong DTO
                //
                // Những variation "cũ" mà DTO vẫn giữ lại sẽ là những thằng có VariationId > 0.
                var keepOldIds = dto.Variations
                    .Where(v => v.VariationId > 0)
                    .Select(v => v.VariationId)
                    .ToHashSet();

                // Mọi variation tồn tại TRƯỚC KHI UPDATE nhưng không còn trong keepOldIds => xoá
                foreach (var oldVarId in existingVarIdsBefore)
                {
                    if (!keepOldIds.Contains(oldVarId))
                    {
                        await _variationRepository.DeleteAsync(oldVarId);
                    }
                }
            }

            // 9. Lưu thay đổi cuối cùng xuống DB
            await _productRepository.SaveChangesAsync();

            // 10. Trả lại DTO chi tiết sau update (Brand, Status, Categories, Variations...)
            return await GetProductByIdAsync(id);
        }

    }
}