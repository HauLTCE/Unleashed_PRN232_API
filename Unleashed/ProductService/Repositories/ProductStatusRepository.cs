using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Repositories.IRepositories;
using System;

namespace ProductService.Repositories
{
    public class ProductStatusRepository : IProductStatusRepository
    {
        private readonly ProductDbContext _context;

        public ProductStatusRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ProductStatus>> GetAllAsync()
        {
            return await _context.ProductStatuses.ToListAsync();
        }

        public async Task<ProductStatus?> GetByIdAsync(int id)
        {
            return await _context.ProductStatuses.FindAsync(id);
        }
    }
}
