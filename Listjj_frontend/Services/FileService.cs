using Listjj.Abstract;
using Listjj.Data;
using Listjj.Infrastructure.ViewModels;
using Microsoft.JSInterop;
using IFileService = Listjj_frontend.Services.Abstract.IFileService;

namespace Listjj_frontend.Services
{
    public class FileService : IFileService
    {
        private readonly IApiClient apiClient;
        public FileService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }

        public async Task<FileViewModel> GetFile(Guid id)
        {
            var response = await apiClient.Get<FileViewModel>($"https://localhost:5001/api/file/file_by_id?id={id}");
            var file = response.HttpResponse.IsSuccessStatusCode ? response.Result : new FileViewModel();
            return file;
        }
    }
}
