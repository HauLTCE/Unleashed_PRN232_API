using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Models;

namespace CartService.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllAsync();
        Task<Cart> GetByIdAsync(Guid userId, int variationId);
        Task AddAsync(Cart cart);
        void Update(Cart cart);
        void Remove(Cart cart);
        Task<bool> SaveChangesAsync();
        Task<bool> CartExistsAsync(Guid userId, int variationId);
    }
}