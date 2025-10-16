using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Models;

namespace CartService.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<(Guid,int),Cart>
    {
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(Guid userId);
        // Thêm phương thức mới
        Task<bool> DeleteAllByUserIdAsync(Guid userId);

    }
}