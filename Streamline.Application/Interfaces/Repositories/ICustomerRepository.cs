using Streamline.Domain.Entities.Customers;
using System.Threading.Tasks;

namespace Streamline.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);
        
        Task<Customer?> GetById(int id);
        Task<int> SaveChangesAsync();

        Task<bool> PhoneExists(string phone);
        Task<bool> EmailExists(string email);
        Task<bool> DocumentExists(string document);
    }
}
