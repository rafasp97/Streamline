using Streamline.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Application.Repositories;
using System.Threading.Tasks;
using Streamline.Domain.Enums;

namespace Streamline.Infrastructure.Persistence.SqlServer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SqlServerDbContext _context;

        public OrderRepository(SqlServerDbContext context)
        {
            _context = context;
        }

        public void Add(Order order)
        {
            _context.Order.Add(order);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAll(
            EStatusOrder? status,
            int? customerId,
            DateTime? createdFrom,
            DateTime? createdTo)
        {
            var query = _context.Order
                .Include(o => o.Customer)
                .ThenInclude(c => c.Contact)
                .Include(o => o.OrderProduct)
                .ThenInclude(op => op.Product)
                .Where(o => o.DeletedAt == null)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (createdFrom.HasValue)
                query = query.Where(o => o.CreatedAt >= createdFrom.Value);

            if (createdTo.HasValue)
                query = query.Where(o => o.CreatedAt <= createdTo.Value);

            return await query.ToListAsync();
        }

        public async Task<Order?> GetById(int id)
        {
            return await _context.Order
                .Include(o => o.Customer)
                .ThenInclude(c => c.Contact)
                .Include(o => o.OrderProduct)
                .ThenInclude(op => op.Product)
                .Where(o => o.DeletedAt == null)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

    }
}
