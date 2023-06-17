using Listjj.Infrastructure.ViewModels;
using ListjjFrontEnd.Services.Abstract;
using Microsoft.JSInterop;

namespace ListjjFrontEnd.Services
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

        public async Task<FileSimpleViewModel> GetFileWithoutBytes(Guid id)
        {
            var response = await apiClient.Get<FileSimpleViewModel>($"/api/file/get_file_simple_by_id?id={id}");
            var file = response.HttpResponse.IsSuccessStatusCode ? response.Result : new FileSimpleViewModel();
            return file;
        }

        public async Task<bool> AddFile(FileViewModel file)
        {
            var response = await apiClient.Post<FileViewModel, bool>($"/api/file/add", file);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFile(Guid id)
        {
            var response = await apiClient.Post<Guid, bool>($"/api/file/delete", id);
            return response.HttpResponse.IsSuccessStatusCode;
        }
        public async Task<FileViewModel> GetFile(Guid id)
        {
            var response = await apiClient.Get<FileViewModel>($"/api/file/get_by_id?id={id}");
            var file = response.HttpResponse.IsSuccessStatusCode ? response.Result : new FileViewModel();
            return file;
        }
    }
}
