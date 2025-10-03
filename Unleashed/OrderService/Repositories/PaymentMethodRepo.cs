using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class PaymentMethodRepo : IPaymenMethodRepo
    {
        private readonly OrderDbContext _context;   
        public PaymentMethodRepo(OrderDbContext context)
        {
            _context = context;
        }
        public IQueryable<PaymentMethod> All()
        {
            return _context.PaymentMethods.AsQueryable();
        }

        public async Task<bool> CreateAsync(PaymentMethod entity)
        {
            try 
            {
                await _context.PaymentMethods.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(PaymentMethod entity)
        {
            try 
            {
                _context.PaymentMethods.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PaymentMethod?> FindAsync(int id)
        {
            return await _context.PaymentMethods.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _context.PaymentMethods.AnyAsync(pm => pm.PaymentMethodId == id);
        }

        public async Task<bool> SaveAsync()
        {
            try 
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(PaymentMethod entity)
        {
            try 
            {
                _context.PaymentMethods.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
