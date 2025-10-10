﻿using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.Common;
using ProductService.Models;
using ProductService.Repositories.Extensions;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class VariationRepository : IVariationRepository
    {
        private readonly ProductDbContext _context;

        public VariationRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Variation>> GetPagedByProductIdAsync(Guid productId, PaginationParams pagination)
        {
            var query = _context.Variations
                .Where(v => v.ProductId == productId)
                .Include(v => v.Size)
                .Include(v => v.Color)
                .OrderBy(v => v.VariationId);

            return await query.ToPagedResultAsync(pagination.PageNumber, pagination.PageSize);
        }
        public async Task<List<Variation>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Variations
                .Where(v => v.ProductId == productId)
                .Include(v => v.Size)
                .Include(v => v.Color)
                .OrderBy(v => v.VariationId)
                .ToListAsync();
        }

        public async Task<Variation?> GetByIdAsync(int id)
            => await _context.Variations
                .Include(v => v.Size)
                .Include(v => v.Color)
                .FirstOrDefaultAsync(v => v.VariationId == id);

        public async Task<Variation> CreateAsync(Variation variation)
        {
            _context.Variations.Add(variation);
            await _context.SaveChangesAsync();
            return variation;
        }

        public async Task<Variation?> UpdateAsync(Variation variation)
        {
            _context.Variations.Update(variation);
            await _context.SaveChangesAsync();
            return variation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variation = await _context.Variations.FindAsync(id);
            if (variation == null) return false;

            _context.Variations.Remove(variation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Variations.AnyAsync(v => v.VariationId == id);

        public async Task<bool> DeleteByProductIdAsync(Guid productId)
        {
            var list = await _context.Variations.Where(v => v.ProductId == productId).ToListAsync();
            if (list.Count == 0) return false;
            _context.Variations.RemoveRange(list);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Variation>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idArr = ids?.Distinct().ToArray() ?? Array.Empty<int>();
            if (idArr.Length == 0) return new List<Variation>();

            return await _context.Variations
                .Where(v => idArr.Contains(v.VariationId))
                .Include(v => v.Size)
                .Include(v => v.Color)
                .Include(v => v.Product)
                .OrderBy(v => v.VariationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Variation>> SearchAsync(string? search, Guid? productId, int? colorId, int? sizeId)
        {
            var q = _context.Variations
                .Include(v => v.Size)
                .Include(v => v.Color)
                .Include(v => v.Product)
                .AsNoTracking()
                .AsQueryable();

            if (productId.HasValue) q = q.Where(v => v.ProductId == productId.Value);
            if (colorId.HasValue) q = q.Where(v => v.ColorId == colorId.Value);
            if (sizeId.HasValue) q = q.Where(v => v.SizeId == sizeId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(v =>
                    (v.Product != null && (
                        v.Product.ProductName.Contains(s) ||
                        (v.Product.ProductCode ?? string.Empty).Contains(s)))
                    || (v.Color != null && v.Color.ColorName.Contains(s))
                    || (v.Size != null && v.Size.SizeName.Contains(s))
                );
            }

            return await q.OrderByDescending(v => v.VariationId).ToListAsync();
        }
    }
}
