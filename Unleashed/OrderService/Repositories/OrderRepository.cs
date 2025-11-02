using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dtos;
using OrderService.Models;
using OrderService.Repositories.Interfaces;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> All()
        {
            return _context.Orders.AsQueryable();
        }

        public async Task<bool> CreateAsync(Order entity)
        {
            try 
            {
                await _context.Orders.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Order entity)
        {
            try 
            {
                _context.Orders.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Order?> FindAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderVariations)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders
                .Where(o => o.UserId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(int statusId)
        {
            return await _context.Orders
                .Where(o => o.OrderStatusId == statusId)
                .ToListAsync();
        }

        public async Task<bool> IsAny(Guid id)
        {
            return await _context.Orders.AnyAsync(e => e.OrderId == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool Update(Order entity)
        {
            try 
            {
                entity.OrderUpdatedAt = DateTimeOffset.UtcNow;
                _context.Orders.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Order?> GetOrderDetailsByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Include(o => o.ShippingMethod)
                .Include(o => o.OrderVariations)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<PagedResult<Order>> GetOrdersAsync(string? search, string? sort, int? statusId, int page, int size)
        {
            var query = _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .AsQueryable();

            // Lọc theo trạng thái
            if (statusId.HasValue)
            {
                query = query.Where(o => o.OrderStatusId == statusId.Value);
            }

            // Lọc theo từ khóa tìm kiếm (ví dụ tìm theo mã đơn hàng hoặc tên khách hàng - cần join thêm)
            if (!string.IsNullOrEmpty(search))
            {
                // Giả định bạn sẽ join với bảng User để tìm theo tên, hoặc tìm theo mã đơn, mã vận đơn
                // query = query.Where(o => o.OrderId.ToString().Contains(search) || o.User.FullName.Contains(search));
                query = query.Where(o => o.OrderTrackingNumber.Contains(search));
            }

            // Sắp xếp
            switch (sort)
            {
                case "totalPrice_asc":
                    query = query.OrderBy(o => o.OrderTotalAmount);
                    break;
                case "totalPrice_desc":
                    query = query.OrderByDescending(o => o.OrderTotalAmount);
                    break;
                default:
                    // Sắp xếp ưu tiên PENDING lên đầu, sau đó theo ngày cập nhật mới nhất
                    query = query.OrderBy(o => o.OrderStatus.OrderStatusName == "PENDING" ? 1 : 2)
                                 .ThenByDescending(o => o.OrderUpdatedAt);
                    break;
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip(page * size).Take(size).ToListAsync();

            return new PagedResult<Order> { Items = items, TotalItems = totalItems };
        }

        public async Task<(IEnumerable<Order>, int total)> GetOrdersByUserIdAsync(Guid userId, int page, int size)
        {
            var query = _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.PaymentMethod)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return  (items,totalItems);
        }
    }
}