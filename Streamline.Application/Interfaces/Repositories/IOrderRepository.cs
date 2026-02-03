using Streamline.Domain.Entities.Orders;
using Streamline.Domain.Enums;
using System.Threading.Tasks;

namespace Streamline.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        Task<int> SaveChanges();
        Task<(int total, List<Order> items)>  GetAll(int page, int limit, EStatusOrder? status, int? customerId, DateTime? createdFrom, DateTime? createdTo);
        Task<Order?> GetById(int Id);
        Task Update(Order order);
    }
}
