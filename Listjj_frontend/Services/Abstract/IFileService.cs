using Listjj.Infrastructure.ViewModels;

namespace Listjj_frontend.Services.Abstract
{
    public interface IFileService
    {
        Task<FileViewModel> GetFile(Guid id);
    }
}
