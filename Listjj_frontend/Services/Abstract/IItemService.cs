using Listjj.Infrastructure.ViewModels;

namespace Listjj_frontend.Services.Abstract
{
    public interface IItemService
    {
        Task<bool> AddorUpdateItem(ListItemViewModel item);
        Task<bool> DeleteItem(Guid id);
        Task<ListItemViewModel> GetById(Guid id);
        Task<List<ListItemViewModel>> GetItemsByCategoryId(Guid categoryId);
        Task<List<ListItemViewModel>> GetAllByUserId(Guid userId);
        Task<bool> Move(Guid id, string direction);
    }
}
