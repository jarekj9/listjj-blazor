using Listjj.Infrastructure.ViewModels;
using Microsoft.JSInterop;

namespace ListjjFrontEnd.Services.Abstract
{
    public interface IFileService
    {
        Task SaveAs(IJSRuntime js, string filename, byte[] data);
        Task<bool> AddFile(FileViewModel file);
        Task<FileSimpleViewModel> GetFileWithoutBytes(Guid id);
        Task<bool> DeleteFile(Guid id);
        Task<FileViewModel> GetFile(Guid id);
    }
}
