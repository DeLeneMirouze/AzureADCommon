using System.Threading.Tasks;

namespace CommonLibrary.Repositories
{
    public interface IArmRepositories
    {
        Task<string> GetArmRequest(string url, string armToken);
    }
}