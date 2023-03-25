using Listjj.Infrastructure.Enums;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Listjj_frontend.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Charts;
using NuGet.Protocol;
using System.Linq.Expressions;
using System.Web;

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
        public async Task<List<ListItemViewModel>> GetItemsByFilter(string searchWords, DateTime? fromDate, DateTime? toDate, Guid categoryId, Guid userId)
        {
            searchWords = HttpUtility.UrlEncode(searchWords);
            var fromDateStr = HttpUtility.UrlEncode(fromDate.ToString());
            var toDateStr = HttpUtility.UrlEncode(toDate.ToString());
            var response = await apiClient.Get<List<ListItemViewModel>>(
                $"https://localhost:5001/api/item/items_by_filter?searchWords={searchWords}&fromDateStr={fromDateStr}&toDateStr={toDateStr}&categoryId={categoryId}&userId={userId}");
            var items = response.HttpResponse.IsSuccessStatusCode ? response.Result : new List<ListItemViewModel>();
            return items;
        }

        public async Task<bool> Move(ListItemViewModel movedItem, MoveDirection direction)
        {
            var movedItemSequence = movedItem?.SequenceNumber ?? 0;

            if (direction == MoveDirection.Up)
            {
                var previousItem = (await GetItemsByCategoryId(movedItem.CategoryId))
                    .Where(i => i.SequenceNumber < movedItemSequence)
                    .OrderBy(i => i.SequenceNumber).LastOrDefault();
                var previousItemSequence = previousItem?.SequenceNumber ?? -1;
                if (previousItemSequence == -1)
                {
                    return false;
                }
                movedItem.SequenceNumber = previousItemSequence;
                previousItem.SequenceNumber = movedItemSequence;
            }

            if (direction == MoveDirection.Down)
            {
                var nextItem = (await GetItemsByCategoryId(movedItem.CategoryId))
                    .Where(i => i.SequenceNumber < movedItemSequence)
                    .OrderBy(i => i.SequenceNumber).FirstOrDefault();
                var nextItemSequence = nextItem?.SequenceNumber ?? -1;
                if (nextItemSequence == -1)
                {
                    return false;
                }
                movedItem.SequenceNumber = nextItemSequence;
                nextItem.SequenceNumber = movedItemSequence;
            }

            var response = await AddorUpdateItem(movedItem);
            return response;
        }

    }
}
