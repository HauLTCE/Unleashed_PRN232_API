using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.DTOs.BrandDTOs;
using ProductService.DTOs.Common;
using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BrandService> _logger;

        public BrandService(
            IBrandRepository brandRepository,
            IMapper mapper,
            ILogger<BrandService> logger)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<BrandDetailDTO>> GetPagedBrandsAsync(PaginationParams pagination)
        {
            var pagedBrands = await _brandRepository.GetPagedAsync(pagination);
            var dtoItems = _mapper.Map<List<BrandDetailDTO>>(pagedBrands.Items);

            return new PagedResult<BrandDetailDTO>
            {
                Items = dtoItems,
                TotalCount = pagedBrands.TotalCount,
                PageNumber = pagedBrands.PageNumber,
                PageSize = pagedBrands.PageSize
            };
        }

        public async Task<BrandDetailDTO?> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand == null ? null : _mapper.Map<BrandDetailDTO>(brand);
        }

        public async Task<BrandDetailDTO> CreateBrandAsync(CreateBrandDTO createBrandDto)
        {
            var brand = _mapper.Map<Brand>(createBrandDto);
            brand.BrandCreatedAt = DateTimeOffset.UtcNow;
            brand.BrandUpdatedAt = DateTimeOffset.UtcNow;

            var created = await _brandRepository.CreateAsync(brand);
            return _mapper.Map<BrandDetailDTO>(created);
        }

        public async Task<BrandDetailDTO?> UpdateBrandAsync(int id, UpdateBrandDTO updateBrandDto)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null) return null;

            brand.BrandName = updateBrandDto.BrandName ?? brand.BrandName;
            brand.BrandDescription = updateBrandDto.BrandDescription ?? brand.BrandDescription;
            brand.BrandImageUrl = updateBrandDto.BrandImageUrl ?? brand.BrandImageUrl;
            brand.BrandWebsiteUrl = updateBrandDto.BrandWebsiteUrl ?? brand.BrandWebsiteUrl;
            brand.BrandUpdatedAt = DateTimeOffset.UtcNow;

            var updated = await _brandRepository.UpdateAsync(brand);
            return updated == null ? null : _mapper.Map<BrandDetailDTO>(updated);
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            return await _brandRepository.DeleteAsync(id);
        }
    }
}