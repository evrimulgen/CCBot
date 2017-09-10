using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IApiRepository
    {
        bool IsHealthy();
        Task<bool> FillRepository();
    }
}