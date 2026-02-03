using Streamline.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Application.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Streamline.Infrastructure.Persistence.SqlServer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly SqlServerDbContext _context;

        public ProductRepository(SqlServerDbContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Product.Add(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetById(int id)
        {
            return await _context.Product
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
