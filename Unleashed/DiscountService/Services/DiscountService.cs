using AutoMapper;
using DiscountService.DTOs;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using DiscountService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        public DiscountService(IDiscountRepository discountRepository, IMapper mapper) {
            _mapper = mapper;
            _discountRepository = discountRepository;
        }

        public async Task<Discount> CreateDiscountAsync(CreateDiscountDto createDiscountDTO)
        {
            var discount = _mapper.Map<Discount>(createDiscountDTO);
            discount.DiscountCreatedAt = DateTimeOffset.UtcNow;
            discount.DiscountUpdatedAt = DateTimeOffset.UtcNow;
            discount.DiscountUsageCount = 0;
            var created = await _discountRepository.CreateAsync(discount);
            if (!created) {
                throw new Exception("Failed to create discount");
            }
            var saved = await _discountRepository.SaveAsync();
            if (!saved) {
                throw new Exception("Failed to save discount");
            }
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var discount = await _discountRepository.FindAsync(id);
            if (discount == null) {
                return false;
            }
            var deleted = _discountRepository.Delete(discount);
            if (!deleted) {
                throw new Exception("Failed to delete discount");
            }
            var saved = await _discountRepository.SaveAsync();
            if (!saved) {
                throw new Exception("Failed to save discount");
            }
            return true;
        }

        public async Task<Discount?> GetDiscountByIdAsync(int id)
        {
            return await _discountRepository.FindAsync(id);
        }

        public async Task<UpdateDiscountDto?> GetDiscountForUpdateAsync(int id) //????????????????
        {
            var discount = await _discountRepository.FindAsync(id);
            if (discount == null) {
                return null;
            }
            return _mapper.Map<UpdateDiscountDto>(discount);
        }

        public async Task<IEnumerable<Discount>> GetDiscountsAsync()
        {
            return await _discountRepository.All().ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetDiscountStatusesAsync() //????????????????????????
        {
            var statuses = await _discountRepository.All()
                .Select(d => new { d.DiscountStatusId, d.DiscountStatus.DiscountStatusName })
                .Distinct()
                .ToListAsync();
            return statuses.Select(s => new SelectListItem
            {
                Value = s.DiscountStatusId.ToString(),
                Text = s.DiscountStatusName
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetDiscountTypesAsync() //?????????????????????????????/
        {
            var types = await _discountRepository.All()
                .Select(d => new { d.DiscountTypeId, d.DiscountType.DiscountTypeName })
                .Distinct()
                .ToListAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.DiscountTypeId.ToString(),
                Text = t.DiscountTypeName
            });
        }

        public async Task<Discount?> UpdateDiscountAsync(int id, UpdateDiscountDto updateDiscountDTO)
        {
            var discount = await _discountRepository.FindAsync(id);
            if (discount == null) {
                return null;
            }
            _mapper.Map(updateDiscountDTO, discount);
            discount.DiscountUpdatedAt = DateTimeOffset.UtcNow;
            var updated = _discountRepository.Update(discount);
            if (!updated) {
                throw new Exception("Failed to update discount");
            }
            var saved = await _discountRepository.SaveAsync();
            if (!saved) {
                throw new Exception("Failed to save discount");
            }
            return discount;
        }
    }
}
