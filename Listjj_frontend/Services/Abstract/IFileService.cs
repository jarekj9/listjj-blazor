using Listjj.Infrastructure.ViewModels;
using Microsoft.JSInterop;

namespace Listjj_frontend.Services.Abstract
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
