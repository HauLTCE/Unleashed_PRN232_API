using AutoMapper;
using DiscountService.Data;
using DiscountService.DTOs;
using DiscountService.Models;
using DiscountService.Repositories.Interfaces;
using DiscountService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscountService.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepo;
        private readonly IUserDiscountRepository _userDiscountRepo;
        private readonly IDiscountStatusRepository _statusRepo;
        private readonly IDiscountTypeRepository _typeRepo;
        private readonly IMapper _mapper;

        public DiscountService(
            IDiscountRepository discountRepo,
            IUserDiscountRepository userDiscountRepo,
            IDiscountStatusRepository statusRepo,
            IDiscountTypeRepository typeRepo,
            IMapper mapper)
        {
            _discountRepo = discountRepo;
            _userDiscountRepo = userDiscountRepo;
            _statusRepo = statusRepo;
            _typeRepo = typeRepo;
            _mapper = mapper;
        }

        #region Tác vụ nền tự động
        public async Task PerformScheduledStatusUpdatesAsync()
        {
            await UpdateDiscountsToActiveAsync();
            await UpdateDiscountsToInactiveOnUsageAsync();
            await UpdateDiscountsToExpiredAsync();
            await _discountRepo.SaveAsync();
        }

        private async Task UpdateDiscountsToActiveAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var discountsToActivate = await _discountRepo.GetDiscountsToActivateAsync(1 /* INACTIVE */, now);
            foreach (var discount in discountsToActivate)
            {
                if (discount.DiscountUsageLimit == null || discount.DiscountUsageCount < discount.DiscountUsageLimit)
                {
                    discount.DiscountStatusId = 2; // ACTIVE
                    _discountRepo.Update(discount);
                }
            }
        }

        private async Task UpdateDiscountsToInactiveOnUsageAsync()
        {
            var activeDiscounts = await _discountRepo.GetActiveDiscountsWithUsageLimitAsync(2 /* ACTIVE */);
            var discountsToInactivate = activeDiscounts
                .Where(d => d.DiscountUsageCount >= d.DiscountUsageLimit)
                .ToList();
            foreach (var discount in discountsToInactivate)
            {
                discount.DiscountStatusId = 1; // INACTIVE
                _discountRepo.Update(discount);
            }
        }

        private async Task UpdateDiscountsToExpiredAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var discountsToExpire = await _discountRepo.GetDiscountsToExpireAsync(3 /* EXPIRED */, now);
            foreach (var discount in discountsToExpire)
            {
                discount.DiscountStatusId = 3; // EXPIRED
                _discountRepo.Update(discount);
            }
        }

        private void SetInitialDiscountStatus(Discount discount)
        {
            var now = DateTimeOffset.UtcNow;
            if (discount.DiscountStartDate <= now && discount.DiscountEndDate > now)
            {
                discount.DiscountStatusId = 2; // ACTIVE
            }
            else
            {
                discount.DiscountStatusId = 1; // INACTIVE
            }
        }
        #endregion

        #region CRUD và Quản lý Discount
        public async Task<DiscountViewDto> CreateDiscountAsync(CreateDiscountDto createDto)
        {
            if (createDto.DiscountStartDate.HasValue && createDto.DiscountEndDate.HasValue &&
                createDto.DiscountStartDate > createDto.DiscountEndDate)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            var discount = _mapper.Map<Discount>(createDto);
            SetInitialDiscountStatus(discount);
            discount.DiscountUsageCount = 0;
            discount.DiscountCreatedAt = DateTimeOffset.UtcNow;
            discount.DiscountUpdatedAt = DateTimeOffset.UtcNow;

            await _discountRepo.CreateAsync(discount);
            await _discountRepo.SaveAsync();

            var result = await _discountRepo.FindAsync(discount.DiscountId);
            return _mapper.Map<DiscountViewDto>(result);
        }

        public async Task<DiscountViewDto?> UpdateDiscountAsync(int id, UpdateDiscountDto updateDto)
        {
            var existingDiscount = await _discountRepo.FindAsync(id);
            if (existingDiscount == null) return null;

            _mapper.Map(updateDto, existingDiscount);
            SetInitialDiscountStatus(existingDiscount);
            existingDiscount.DiscountUpdatedAt = DateTimeOffset.UtcNow;

            _discountRepo.Update(existingDiscount);
            await _discountRepo.SaveAsync();

            return _mapper.Map<DiscountViewDto>(existingDiscount);
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var discount = await _discountRepo.FindAsync(id);
            if (discount == null) return false;

            _discountRepo.Delete(discount);
            return await _discountRepo.SaveAsync();
        }

        public async Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetAllDiscountsAsync(string? search, int? statusId, int? typeId, int page, int size)
        {
            // SỬA Ở ĐÂY: Khai báo rõ kiểu là IQueryable<Discount> thay vì dùng 'var'
            IQueryable<Discount> query = _discountRepo.All()
                .Include(d => d.DiscountStatus)
                .Include(d => d.DiscountType);

            if (!string.IsNullOrEmpty(search))
            {
                // Bây giờ phép gán này sẽ hợp lệ
                query = query.Where(d => d.DiscountCode.Contains(search) || (d.DiscountDescription != null && d.DiscountDescription.Contains(search)));
            }
            if (statusId.HasValue)
            {
                query = query.Where(d => d.DiscountStatusId == statusId.Value);
            }
            if (typeId.HasValue)
            {
                query = query.Where(d => d.DiscountTypeId == typeId.Value);
            }

            var totalRecords = await query.CountAsync();
            var data = await query.OrderByDescending(d => d.DiscountId)
                                .Skip(page * size)
                                .Take(size)
                                .ToListAsync();

            return (_mapper.Map<IEnumerable<DiscountViewDto>>(data), totalRecords);
        }

        public async Task<DiscountViewDto?> GetDiscountByIdAsync(int id)
        {
            var discount = await _discountRepo.All()
                                        .Include(d => d.DiscountStatus)
                                        .Include(d => d.DiscountType)
                                        .FirstOrDefaultAsync(d => d.DiscountId == id);
            return _mapper.Map<DiscountViewDto>(discount);
        }

        public async Task<bool> IsDiscountCodeExistsAsync(string code)
        {
            return await _discountRepo.All().AnyAsync(d => d.DiscountCode == code);
        }
        #endregion

        #region Quản lý User cho Discount
        public async Task AddUsersToDiscountAsync(int discountId, List<string> userIds)
        {
            var discountExists = await _discountRepo.IsAny(discountId);
            if (!discountExists)
            {
                throw new KeyNotFoundException("Discount not found.");
            }

            var userDiscountsToAdd = new List<UserDiscount>();
            foreach (var userIdStr in userIds)
            {
                if (Guid.TryParse(userIdStr, out Guid userGuid))
                {
                    var exists = await _userDiscountRepo.ExistsAsync(userGuid, discountId);
                    if (!exists)
                    {
                        userDiscountsToAdd.Add(new UserDiscount
                        {
                            UserId = userGuid,
                            DiscountId = discountId,
                            IsDiscountUsed = false,
                            DiscountUsedAt = null
                        });
                    }
                }
            }

            if (userDiscountsToAdd.Any())
            {
                await _userDiscountRepo.AddRangeAsync(userDiscountsToAdd);
                await _userDiscountRepo.SaveAsync();
            }
        }

        public async Task RemoveUserFromDiscountAsync(int discountId, string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Invalid User ID format.");
            }

            var userDiscount = await _userDiscountRepo.FindByUserIdAndDiscountIdAsync(userGuid, discountId);
            if (userDiscount != null)
            {
                _userDiscountRepo.Delete(userDiscount);
                await _userDiscountRepo.SaveAsync();
            }
        }

        public async Task<object> GetUsersByDiscountIdAsync(int discountId)
        {
            var userDiscounts = await _userDiscountRepo.All()
                                           .Where(ud => ud.DiscountId == discountId)
                                           .ToListAsync();

            var users = _mapper.Map<List<UserDiscountDto>>(userDiscounts);
            var allowedUserIds = users.Select(u => u.UserId).ToList();

            return new { users, allowedUserIds };
        }
        #endregion

        #region Logic cho người dùng cuối
        public async Task<(IEnumerable<DiscountViewDto> Data, int TotalRecords)> GetDiscountsForUserAsync(string userId, string? search, int? statusId, int? typeId, int page, int size, string? sortBy, string? sortOrder)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Invalid User ID format.");
            }

            var userDiscountIds = await _userDiscountRepo.FindDiscountIdsByUserIdAsync(userGuid);
            if (!userDiscountIds.Any())
            {
                return (Enumerable.Empty<DiscountViewDto>(), 0);
            }

            var query = _discountRepo.All()
                .Include(d => d.DiscountStatus)
                .Include(d => d.DiscountType)
                .Where(d => userDiscountIds.Contains(d.DiscountId));

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.DiscountCode.Contains(search) || (d.DiscountDescription != null && d.DiscountDescription.Contains(search)));
            }
            if (statusId.HasValue)
            {
                query = query.Where(d => d.DiscountStatusId == statusId.Value);
            }
            if (typeId.HasValue)
            {
                query = query.Where(d => d.DiscountTypeId == typeId.Value);
            }

            if (string.Equals(sortBy, "amount", StringComparison.OrdinalIgnoreCase))
            {
                query = sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(d => d.DiscountValue)
                    : query.OrderBy(d => d.DiscountValue);
            }
            else
            {
                query = query.OrderBy(d => d.DiscountStatusId).ThenBy(d => d.DiscountEndDate);
            }

            var totalRecords = await query.CountAsync();
            var data = await query.Skip(page * size).Take(size).ToListAsync();

            return (_mapper.Map<IEnumerable<DiscountViewDto>>(data), totalRecords);
        }

        public async Task<DiscountViewDto?> GetDiscountForUserByIdAsync(int discountId, string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Invalid User ID format.");
            }

            var isAssigned = await _userDiscountRepo.ExistsAsync(userGuid, discountId);
            if (!isAssigned) return null;

            return await GetDiscountByIdAsync(discountId);
        }

        public async Task<object> CheckUserDiscountAsync(string discountCode, decimal subTotal, ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !Guid.TryParse(userIdStr, out Guid userId))
            {
                return new { IsSuccess = false, StatusCode = 401, Message = "User not authenticated." };
            }

            var discount = await _discountRepo.FindByCodeAsync(discountCode);
            if (discount == null)
            {
                return new { IsSuccess = false, StatusCode = 404, Message = "Discount code not found." };
            }

            var userDiscount = await _userDiscountRepo.FindByUserIdAndDiscountIdAsync(userId, discount.DiscountId);
            if (userDiscount == null)
            {
                return new { IsSuccess = false, StatusCode = 403, Message = "User is not eligible for this discount." };
            }
            if (userDiscount.IsDiscountUsed)
            {
                return new { IsSuccess = false, StatusCode = 410, Message = "This discount code has already been used." };
            }
            if (discount.DiscountStatusId != 2 /* ACTIVE */)
            {
                return new { IsSuccess = false, StatusCode = 410, Message = "Discount is not active." };
            }
            if (discount.DiscountMinimumOrderValue.HasValue && subTotal < discount.DiscountMinimumOrderValue.Value)
            {
                return new { IsSuccess = false, StatusCode = 400, Message = $"Minimum order value of {discount.DiscountMinimumOrderValue:C} is required." };
            }

            return new { IsSuccess = true, StatusCode = 200, Data = _mapper.Map<DiscountViewDto>(discount) };
        }

        public async Task UpdateUsageCountAsync(string discountCode, string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Invalid User ID format.");
            }

            var discount = await _discountRepo.FindByCodeAsync(discountCode);
            if (discount == null) throw new KeyNotFoundException("Discount code not found.");

            var userDiscount = await _userDiscountRepo.FindByUserIdAndDiscountIdAsync(userGuid, discount.DiscountId);
            if (userDiscount == null) throw new InvalidOperationException("User has not been assigned this discount.");
            if (userDiscount.IsDiscountUsed) throw new InvalidOperationException("User has already used this discount.");

            userDiscount.IsDiscountUsed = true;
            userDiscount.DiscountUsedAt = DateTimeOffset.UtcNow;
            _userDiscountRepo.Update(userDiscount);

            discount.DiscountUsageCount = (discount.DiscountUsageCount ?? 0) + 1;
            if (discount.DiscountUsageLimit.HasValue && discount.DiscountUsageCount >= discount.DiscountUsageLimit)
            {
                discount.DiscountStatusId = 1; // INACTIVE
            }
            _discountRepo.Update(discount);

            await _discountRepo.SaveAsync();
        }

        public async Task<IEnumerable<DiscountViewDto>> GetBestDiscountsForCheckoutAsync(string userId, decimal cartTotal)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                throw new ArgumentException("Invalid User ID format.");
            }

            var userDiscounts = await _userDiscountRepo.FindByUserIdAsync(userGuid);
            var unusedDiscountIds = userDiscounts.Where(ud => !ud.IsDiscountUsed).Select(ud => ud.DiscountId).ToList();

            if (!unusedDiscountIds.Any()) return Enumerable.Empty<DiscountViewDto>();

            var potentialDiscounts = await _discountRepo.All()
                .Where(d => unusedDiscountIds.Contains(d.DiscountId))
                .Include(d => d.DiscountStatus)
                .Include(d => d.DiscountType)
                .ToListAsync();

            var applicableDiscounts = potentialDiscounts
                .Where(d => d.DiscountStatusId == 2 /* ACTIVE */ &&
                            (d.DiscountMinimumOrderValue == null || cartTotal >= d.DiscountMinimumOrderValue))
                .ToList();

            var discountsWithSavings = applicableDiscounts.Select(discount => {
                decimal savings = 0;
                if (discount.DiscountTypeId == 1 /* PERCENTAGE */ && discount.DiscountValue.HasValue)
                {
                    decimal potentialSavings = cartTotal * (discount.DiscountValue.Value / 100m);
                    savings = discount.DiscountMaximumValue.HasValue && potentialSavings > discount.DiscountMaximumValue.Value
                        ? discount.DiscountMaximumValue.Value
                        : potentialSavings;
                }
                else if (discount.DiscountTypeId == 2 /* FLAT_AMOUNT */ && discount.DiscountValue.HasValue)
                {
                    savings = discount.DiscountValue.Value;
                }
                return new { Discount = discount, Savings = savings };
            });

            var bestDiscounts = discountsWithSavings
                .OrderByDescending(x => x.Savings)
                .Take(5)
                .Select(x => x.Discount);

            return _mapper.Map<IEnumerable<DiscountViewDto>>(bestDiscounts);
        }
        #endregion

        #region Lấy danh sách phụ
        public async Task<IEnumerable<DiscountStatus>> GetAllStatusesAsync()
        {
            return await _statusRepo.All().ToListAsync();
        }

        public async Task<IEnumerable<DiscountType>> GetAllTypesAsync()
        {
            return await _typeRepo.All().ToListAsync();
        }
        #endregion
    }
}