
namespace ListjjFrontEnd.Services.Abstract
{
    public interface ITagsService
    {
        Task<List<string>> GetByUserId();
        Task<bool> UpdateByUserId(List<string> tags);
    }
}
