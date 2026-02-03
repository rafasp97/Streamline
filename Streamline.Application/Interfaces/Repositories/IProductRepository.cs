using Streamline.Domain.Entities.Products;
using System.Threading.Tasks;

namespace Streamline.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);

        Task<int> SaveChangesAsync();
        Task<Product?> GetById(int id);
        
    }
}
