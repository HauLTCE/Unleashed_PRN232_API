using AutoMapper;
using DiscountService.DTOs;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using DiscountService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DiscountService.Services
{
    public class DiscountService : IDiscountService
    {
        public Task AddUsersToDiscountAsync(int discountId, List<string> userIds)
        {
            throw new NotImplementedException();
        }

        public Task<object> CheckUserDiscountAsync(string discountCode, decimal subTotal, ClaimsPrincipal user)
        {
            throw new NotImplementedException();
        }

        public Task<DiscountViewDto> CreateDiscountAsync(CreateDiscountDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDiscountAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetAllDiscountsAsync(string? search, int? statusId, int? typeId, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiscountStatus>> GetAllStatusesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiscountType>> GetAllTypesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DiscountViewDto>> GetBestDiscountsForCheckoutAsync(string userId, decimal cartTotal)
        {
            throw new NotImplementedException();
        }

        public Task<DiscountViewDto?> GetDiscountByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DiscountViewDto?> GetDiscountForUserByIdAsync(int discountId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetDiscountsForUserAsync(string userId, string? search, int? statusId, int? typeId, int page, int size, string? sortBy, string? sortOrder)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetUsersByDiscountIdAsync(int discountId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsDiscountCodeExistsAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task PerformScheduledStatusUpdatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromDiscountAsync(int discountId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<DiscountViewDto?> UpdateDiscountAsync(int id, UpdateDiscountDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUsageCountAsync(string discountCode, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
