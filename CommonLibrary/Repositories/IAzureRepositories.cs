using System.Threading.Tasks;

namespace CommonLibrary.Repositories
{
    public interface IAzureRepositories
    {
        Task<string> GetArmRequest(string urlPath, string armToken);
        Task<string> GetGraphRequest(string urlPath, string graphToken);
    }
}