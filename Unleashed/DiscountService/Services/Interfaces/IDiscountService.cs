// Services/Interfaces/IDiscountService.cs
using DiscountService.DTOs;
using DiscountService.Models;
using System.Security.Claims;

namespace DiscountService.Services.Interfaces
{
    public interface IDiscountService
    {
        // CRUD và Quản lý
        Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetAllDiscountsAsync(string? search, int? statusId, int? typeId, int page, int size);
        Task<DiscountViewDto?> GetDiscountByIdAsync(int id);
        Task<DiscountViewDto> CreateDiscountAsync(CreateDiscountDto createDto);
        Task<DiscountViewDto?> UpdateDiscountAsync(int id, UpdateDiscountDto updateDto);
        Task<bool> DeleteDiscountAsync(int id);
        Task<bool> IsDiscountCodeExistsAsync(string code);

        // Quản lý người dùng
        Task AddUsersToDiscountAsync(int discountId, List<string> userIds);
        Task RemoveUserFromDiscountAsync(int discountId, string userId);
        Task<object> GetUsersByDiscountIdAsync(int discountId);

        // Logic nghiệp vụ cho User/Checkout
        Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetDiscountsForUserAsync(string userId, string? search, int? statusId, int? typeId, int page, int size, string? sortBy, string? sortOrder);
        Task<DiscountViewDto?> GetDiscountForUserByIdAsync(int discountId, string userId);
        Task<object> CheckUserDiscountAsync(string discountCode, decimal subTotal, ClaimsPrincipal user);
        Task UpdateUsageCountAsync(string discountCode, string userId);
        Task<IEnumerable<DiscountViewDto>> GetBestDiscountsForCheckoutAsync(string userId, decimal cartTotal);

        // Tác vụ tự động
        Task PerformScheduledStatusUpdatesAsync();

        // Lấy danh sách phụ
        Task<IEnumerable<DiscountStatus>> GetAllStatusesAsync();
        Task<IEnumerable<DiscountType>> GetAllTypesAsync();
    }
}