using Ardalis.Specification;
using InventoryService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InventoryService.Specifications
{
    public class TransactionSpecification : Specification<Transaction>
    {
        public TransactionSpecification(string? searchTerm, string? dateFilter, string? sort, int page, int pageSize)
        {
            ApplyFilters(searchTerm, dateFilter);
            ApplySorting(sort);
            ApplyPaging(page, pageSize);
        }

        public TransactionSpecification(string? searchTerm, string? dateFilter)
        {
            ApplyFilters(searchTerm, dateFilter);
        }

        private void ApplyFilters(string? searchTerm, string? dateFilter)
        {
            // date filter
            if (!string.IsNullOrEmpty(dateFilter) && !dateFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var now = DateTimeOffset.UtcNow;
                DateTimeOffset startDate = dateFilter.ToLower() switch
                {
                    "today" => new DateTimeOffset(now.Date, now.Offset),
                    "week" => now.AddDays(-7),
                    "month" => now.AddMonths(-1),
                    "6months" => now.AddMonths(-6),
                    _ => DateTimeOffset.MinValue
                };

                if (startDate > DateTimeOffset.MinValue)
                {
                    Query.Where(t => t.TransactionDate >= startDate);
                }
            }

            // search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                Query.Where(t =>
                    (t.Provider != null && t.Provider.ProviderName != null && t.Provider.ProviderName.ToLower().Contains(lowerSearchTerm)) ||
                    (t.Stock != null && t.Stock.StockName != null && t.Stock.StockName.ToLower().Contains(lowerSearchTerm)) ||
                    (t.TransactionType != null && t.TransactionType.TransactionTypeName != null && t.TransactionType.TransactionTypeName.ToLower().Contains(lowerSearchTerm))
                );
            }

            // eager load
            Query.Include(t => t.Provider)
                 .Include(t => t.Stock)
                 .Include(t => t.TransactionType);
        }

        private void ApplySorting(string? sort)
        {
            bool isNewestFirst = string.Equals(sort, "newest_first", StringComparison.OrdinalIgnoreCase);

            if (isNewestFirst)
            {
                Query.OrderByDescending(t => t.TransactionDate);
            }
            else
            {
                Query.OrderBy(t => t.TransactionDate);
            }
        }

        private void ApplyPaging(int page, int pageSize)
        {
            Query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
