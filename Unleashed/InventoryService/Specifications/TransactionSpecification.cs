using Ardalis.Specification;
using InventoryService.Models;

namespace InventoryService.Specifications
{
    public class TransactionSpecification : Specification<Transaction>
    {
        public TransactionSpecification(string? searchTerm, string? dateFilter, string? sort, int page, int pageSize, List<int>? variationIds)
        {
            ApplyPaging(page, pageSize);
            ApplySorting(sort);
            ApplyFilters(searchTerm, dateFilter, variationIds);
        }

        public TransactionSpecification(string? searchTerm, string? dateFilter, List<int>? variationIds)
        {
            ApplyFilters(searchTerm, dateFilter, variationIds);
        }

        private void ApplyFilters(string? searchTerm, string? dateFilter, List<int>? variationIds)
        {
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

            if (variationIds != null && variationIds.Any())
            {
                Query.Where(t => t.VariationId.HasValue && variationIds.Contains(t.VariationId.Value));
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                Query.Where(t =>
                    (t.Provider.ProviderName != null && t.Provider.ProviderName.ToLower().Contains(lowerSearchTerm)) ||
                    (t.Stock.StockName != null && t.Stock.StockName.ToLower().Contains(lowerSearchTerm)) ||
                    (t.TransactionType.TransactionTypeName != null && t.TransactionType.TransactionTypeName.ToLower().Contains(lowerSearchTerm))
                );
            }

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
