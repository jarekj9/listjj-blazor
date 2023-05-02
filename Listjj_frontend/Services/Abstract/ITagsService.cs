
namespace Listjj_frontend.Services.Abstract
{
    public interface ITagsService
    {
        Task<List<string>> GetByUserId(Guid userId);
        Task<bool> UpdateByUserId(Guid userId, List<string> tags);
    }
}
