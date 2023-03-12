using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Services.Abstract;

namespace Listjj_frontend.Services
{
    public class ItemService : IItemService
    {
        private readonly IApiClient apiClient;
        public ItemService(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<bool> AddorUpdateItem(ListItemViewModel item)
        {
            var response = await apiClient.Post<ListItemViewModel, bool>($"https://localhost:5001/api/item/addorupdate", item);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItem(Guid id)
        {
            var response = await apiClient.Post<Guid, bool>($"https://localhost:5001/api/item/delete", id);
            return response.HttpResponse.IsSuccessStatusCode;
        }

        public async Task<ListItemViewModel> GetById(Guid id)
        {
            var response = await apiClient.Get<ListItemViewModel>($"https://localhost:5001/api/item/item_by_id?id={id}");
            var item = response.HttpResponse.IsSuccessStatusCode ? response.Result : new ListItemViewModel();
            return item;
        }

        public async Task<List<ListItemViewModel>> GetItemsByCategoryId(Guid categoryId)
        { 

            var response = await apiClient.Get<List<ListItemViewModel>>($"https://localhost:5001/api/item/items_by_categoryid?categoryId={categoryId}");
            var items = response.HttpResponse.IsSuccessStatusCode? response.Result : new List<ListItemViewModel>();
            return items;
        }

        public async Task<List<ListItemViewModel>> GetAllByUserId(Guid userId)
        {
            var response = await apiClient.Get<List<ListItemViewModel>>($"https://localhost:5001/api/item/items_by_userid?userId={userId}");
            var items = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<ListItemViewModel>();
            return items;
        }

        public async Task<bool> Move(Guid id, string direction)
        {
            var response = await apiClient.Post<(Guid id,string direction), bool>($"https://localhost:5001/api/item/move", (id, direction));
            return response.HttpResponse.IsSuccessStatusCode;
        }

    }
}
