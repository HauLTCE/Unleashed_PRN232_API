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

            // 1) Tạo product (giống code cũ)
            var product = _mapper.Map<Product>(dto);
            product.ProductId = Guid.NewGuid();
            product.ProductCreatedAt = DateTimeOffset.UtcNow;
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.CreateAsync(product);

            var productId = product.ProductId;
            var addedVariationCount = 0;
            var categoriesAdded = dto.CategoryIds?.Any() == true;

            try
            {
                // 2) Gán categories qua Raw SQL (Repo)
                if (dto.CategoryIds?.Any() == true)
                {
                    await _productCategoryRepository.AddCategoriesToProductAsync(productId, dto.CategoryIds);
                }

                // 3) Thêm variations — kiểm tra Size/Color như code cũ
                if (dto.Variations?.Any() == true)
                {
                    foreach (var vdto in dto.Variations)
                    {
                        // check tồn tại (giống code cũ)
                        if (!vdto.SizeId.HasValue || !vdto.ColorId.HasValue) continue;

                        var size = await _sizeRepository.GetByIdAsync(vdto.SizeId.Value);
                        var color = await _colorRepository.GetByIdAsync(vdto.ColorId.Value);
                        if (size == null || color == null) continue;

                        var variation = _mapper.Map<Variation>(vdto);
                        variation.ProductId = productId; // ràng buộc về product vừa tạo

                        await _variationRepository.CreateAsync(variation);
                        addedVariationCount++;
                    }
                }

                // 4) Trả về DTO đầy đủ
                return await GetProductByIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateProduct failed, running compensation. ProductId={ProductId}", productId);

                // —— BÙ TRỪ (Compensation) —— //
                try
                {
                    // Xoá variations đã thêm
                    await _variationRepository.DeleteByProductIdAsync(productId);

                    // Xoá categories liên kết (Raw SQL)
                    await _productCategoryRepository.RemoveAllByProductAsync(productId);

                    // Xoá product
                    await _productRepository.DeleteAsync(productId);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx, "Compensation failed. ProductId={ProductId}", productId);
                }

                throw; // cho controller xử lý trả lỗi 4xx/5xx
            }
        }
        public async Task<ProductDetailDTO?> UpdateProductAsync(Guid id, UpdateProductDTO dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            // 1) UPDATE thuộc tính sản phẩm 
            _mapper.Map(dto, product);
            product.ProductUpdatedAt = DateTimeOffset.UtcNow;

            await _productRepository.UpdateAsync(product);

            // 2) UPDATE Categories 
            if (dto.CategoryIds != null)
            {
                var incoming = dto.CategoryIds.Distinct().ToList();
                var current = (await _productCategoryRepository.GetCategoriesByProductIdAsync(id))
                                .Select(c => c.CategoryId).ToList();

                var toRemove = current.Except(incoming).ToList();
                var toAdd = incoming.Except(current).ToList();

                if (toRemove.Any())
                    await _productCategoryRepository.RemoveCategoriesFromProductAsync(id, toRemove);
                if (toAdd.Any())
                    await _productCategoryRepository.AddCategoriesToProductAsync(id, toAdd);
            }

            // 3) UPSERT Variations 
            if (dto.Variations != null)
            {
                var existing = await _variationRepository.GetByProductIdAsync(id);
                var byId = existing.ToDictionary(v => v.VariationId);
                var keepIds = new HashSet<int>();

                foreach (var vdto in dto.Variations)
                {
                    // Validate nếu có gửi
                    if (vdto.SizeId.HasValue && await _sizeRepository.GetByIdAsync(vdto.SizeId.Value) is null)
                        throw new ArgumentException($"Invalid SizeId={vdto.SizeId.Value}");
                    if (vdto.ColorId.HasValue && await _colorRepository.GetByIdAsync(vdto.ColorId.Value) is null)
                        throw new ArgumentException($"Invalid ColorId={vdto.ColorId.Value}");

                    if (vdto.VariationId > 0 && byId.TryGetValue(vdto.VariationId, out var entity))
                    {
                        // Map bỏ qua null cho entity Variation
                        _mapper.Map(vdto, entity);
                        await _variationRepository.UpdateAsync(entity);
                        keepIds.Add(entity.VariationId);
                    }
                    else
                    {
                        // CREATE cần đủ Size/Color/Price
                        if (!vdto.SizeId.HasValue || !vdto.ColorId.HasValue || !vdto.VariationPrice.HasValue)
                            throw new ArgumentException("New variation requires SizeId, ColorId and VariationPrice");

                        var newVar = _mapper.Map<Variation>(vdto);
                        newVar.ProductId = id;

                        var created = await _variationRepository.CreateAsync(newVar);
                        keepIds.Add(created.VariationId);
                    }
                }

                // DELETE phần còn lại
                var toDelete = existing.Where(x => !keepIds.Contains(x.VariationId))
                                       .Select(x => x.VariationId)
                                       .ToList();
                foreach (var vid in toDelete)
                    await _variationRepository.DeleteAsync(vid);
            }

            // 4) Return DTO đầy đủ
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

        public Task<ProductDetailDTO?> UpdateProductVariationsAsync(Guid productId, List<UpdateVariationDTO> updateVariationDtos)
        {
            throw new NotImplementedException();
        }
    }
}