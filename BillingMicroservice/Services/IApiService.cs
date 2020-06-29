using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BillingMicroservice.Services
{
    public interface IApiService
    {
        Task<string> GetBillingComputeData();

        string GetAccessToken(string jsonKeyFilePath, params string[] scopes);
    }
}
