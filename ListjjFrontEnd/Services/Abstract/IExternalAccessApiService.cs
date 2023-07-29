
namespace ListjjFrontEnd.Services.Abstract
{
    public interface IExternalAccessApiService
    {
        Task<string> GetApiKey();
        Task<string> GenerateApiKey();
    }
}
