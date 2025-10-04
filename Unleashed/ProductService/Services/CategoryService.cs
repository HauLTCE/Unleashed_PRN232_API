using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<CategoryDetailDTO>> GetPagedCategoriesAsync(PaginationParams pagination)
        {
            var pagedCategories = await _categoryRepository.GetPagedAsync(pagination);
            var dtoItems = _mapper.Map<List<CategoryDetailDTO>>(pagedCategories.Items);

            return new PagedResult<CategoryDetailDTO>
            {
                Items = dtoItems,
                TotalCount = pagedCategories.TotalCount,
                PageNumber = pagedCategories.PageNumber,
                PageSize = pagedCategories.PageSize
            };
        }

        public async Task<CategoryDetailDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDetailDTO>(category);
        }

        public async Task<CategoryDetailDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);
            category.CategoryCreatedAt = DateTimeOffset.UtcNow;
            category.CategoryUpdatedAt = DateTimeOffset.UtcNow;

            var created = await _categoryRepository.CreateAsync(category);
            return _mapper.Map<CategoryDetailDTO>(created);
        }

        public async Task<CategoryDetailDTO?> UpdateCategoryAsync(int id, UpdateCategoryDTO updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            category.CategoryName = updateCategoryDto.CategoryName ?? category.CategoryName;
            category.CategoryDescription = updateCategoryDto.CategoryDescription ?? category.CategoryDescription;
            category.CategoryImageUrl = updateCategoryDto.CategoryImageUrl ?? category.CategoryImageUrl;
            category.CategoryUpdatedAt = DateTimeOffset.UtcNow;

            var updated = await _categoryRepository.UpdateAsync(category);
            return updated == null ? null : _mapper.Map<CategoryDetailDTO>(updated);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}