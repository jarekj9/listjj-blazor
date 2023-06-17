using Listjj.Infrastructure.Enums;
using Listjj.Infrastructure.ViewModels;

namespace ListjjFrontEnd.Services.Abstract
{
    public interface IItemService
    {
        Task<bool> AddorUpdateItem(ListItemViewModel item);
        Task<bool> DeleteItem(Guid id);
        Task<ListItemViewModel> GetById(Guid id);
        Task<List<ListItemViewModel>> GetItemsByCategoryId(Guid categoryId);
        Task<List<ListItemViewModel>> GetAllByUserId();
        Task<bool> Move(ListItemViewModel movedItem, MoveDirection direction);
        Task<List<ListItemViewModel>> GetItemsByFilter(string searchWords, DateTime? fromDateStr, DateTime? toDateStr, Guid categoryId);
    }
}
