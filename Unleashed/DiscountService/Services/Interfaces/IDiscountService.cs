using DiscountService.DTOs;
using DiscountService.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscountService.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<IEnumerable<Discount>> GetDiscountsAsync();
        Task<Discount?> GetDiscountByIdAsync(int id);
        Task<Discount> CreateDiscountAsync(CreateDiscountDto createDiscountDTO);
        Task<Discount?> UpdateDiscountAsync(int id, UpdateDiscountDto updateDiscountDTO);
        Task<bool> DeleteDiscountAsync(int id);

        Task<IEnumerable<SelectListItem>> GetDiscountStatusesAsync();
        Task<IEnumerable<SelectListItem>> GetDiscountTypesAsync();
        Task<UpdateDiscountDto?> GetDiscountForUpdateAsync(int id);
    }
}
