using ProductService.Data;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly ProductDbContext _context;

        public SizeRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<Size?> GetByIdAsync(int id)
        => await _context.Sizes.FindAsync(id);

    }
}
