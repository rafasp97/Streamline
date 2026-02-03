using System.Threading.Tasks;

namespace Streamline.Application.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task Low(string message);
        Task Medium(string message);
        Task High(string message);
        Task Critical(string message);
    }
}
