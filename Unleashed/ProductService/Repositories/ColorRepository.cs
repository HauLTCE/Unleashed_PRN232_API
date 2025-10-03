using ProductService.Data;
using ProductService.DTOs.OtherDTOs;
using ProductService.Models;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly ProductDbContext _context;

        public ColorRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<Color?> GetByIdAsync(int id)
         => await _context.Colors.FindAsync(id);
 
    }
}
