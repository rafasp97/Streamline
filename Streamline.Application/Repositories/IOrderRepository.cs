using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Enums;
using System.Threading.Tasks;

namespace Streamline.Application.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        Task<int> SaveChangesAsync();

        Task<List<Order>> GetAll(EStatusOrder? status, int? customerId, DateTime? createdFrom, DateTime? createdTo);
        Task<Order?> GetById(int Id);
    }
}
