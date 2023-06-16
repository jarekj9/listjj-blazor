using AutoMapper;
using Listjj.Abstract;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;  
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Web;
using List.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Listjj.APIs
{
    public class ItemController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ItemController> logger;
        private readonly IMapper mapper;

        public ItemController(IUnitOfWork unitOfWork, ILogger<ItemController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [Authorize]
        [Route("api/[controller]/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllItems()
        {
            var items = await unitOfWork.ListItems.GetAll();
            var itemsVms = mapper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Route("api/[controller]/items_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByUserId(string userId)
        {
            var userIdGuid = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
            var items = await unitOfWork.ListItems.GetAllByUserId(userIdGuid);
            var itemsVms = mapper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Route("api/[controller]/get_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetById(string id)
        {
            var itemIdGuid = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var item = await unitOfWork.ListItems.GetById(itemIdGuid);
            var itemVm = mapper.Map<ListItemViewModel>(item);
            return new JsonResult(itemVm);
        }

        [Route("api/[controller]/items_by_categoryid")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByCategoryId(string categoryId)
        {
            var categoryIdGuid = Guid.TryParse(categoryId, out var guid) ? guid : Guid.Empty;
            var items = await unitOfWork.ListItems.GetAllByCategoryId(categoryIdGuid);
            var itemsVms = mapper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Authorize]
        [Route("api/[controller]/items_by_filter")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByFilter(string searchWords, string fromDateStr, string toDateStr, string categoryId, string userId)
        {
            searchWords = HttpUtility.UrlDecode(searchWords);
            var fromDate =   DateTime.TryParse(HttpUtility.UrlDecode(fromDateStr), out var parsedFrom) ? parsedFrom : DateTime.MinValue;
            var toDate = DateTime.TryParse(HttpUtility.UrlDecode(toDateStr), out var parsedTo) ? parsedTo : DateTime.MaxValue;
            var categoryIdGuid = Guid.TryParse(categoryId, out var parsedCategoryId) ? parsedCategoryId : Guid.Empty;
            var userIdGuid = Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : Guid.Empty;

            var filter = MakeSearchFilter(userIdGuid, categoryIdGuid, fromDate, toDate, searchWords);
            var items = await unitOfWork.ListItems.ExecuteQuery(filter);
            var itemsVms = mapper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }
        private Expression<Func<ListItem, bool>> MakeSearchFilter(Guid userId, Guid categoryId, DateTime fromDate, DateTime toDate, string searchText)
        {
            Expression<Func<ListItem, bool>> filter = x => x.UserId == userId && x.Modified >= fromDate && x.Modified <= toDate;

            if (categoryId != Guid.Empty)
            {
                filter = ExpressionExtensions<ListItem>.AndAlso(filter, x => x.CategoryId == categoryId);
            }

            if (string.IsNullOrEmpty(searchText))
            {
                return filter;
            }
            var searchWords = searchText.Trim().Split(' ').ToList();
            var negatedSearchWords = searchWords.Where(x => x[0] == '!').Select(x => x.Remove(0, 1)).ToList();
            searchWords = searchWords.Where(x => x[0] != '!').ToList();
            filter = searchWords.Aggregate(
                filter,
                (currentExpr, nextWord) => ExpressionExtensions<ListItem>.AndAlso(
                    currentExpr,
                    x => x.Name.Contains(nextWord) || x.Tags.Contains(nextWord)
                )
            );
            filter = negatedSearchWords.Aggregate(
                filter,
                (currentExpr, nextWord) => ExpressionExtensions<ListItem>.AndAlso(
                    currentExpr,
                    x => !x.Name.Contains(nextWord) && !x.Tags.Contains(nextWord)
                )
            );
            return filter;
        }

        [Route("api/[controller]/addorupdate")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateItem([FromBody] ListItemViewModel itemVm)
        {
            var existingItem = await unitOfWork.ListItems.GetById(itemVm.Id);

            if (existingItem != null)
            {
                mapper.Map<ListItemViewModel, ListItem>(itemVm, existingItem);
                unitOfWork.ListItems.Update(existingItem);
            }
            else
            {
                var newItem = mapper.Map<ListItem>(itemVm);
                var allSequenceNumbers = (await unitOfWork.ListItems.GetAllByCategoryId(itemVm.CategoryId)).Select(i => i.SequenceNumber).ToList();
                newItem.SequenceNumber = allSequenceNumbers.Count > 0 ? allSequenceNumbers.Max() + 1 : 1;
                unitOfWork.ListItems.Add(newItem);
            }

            await unitOfWork.Save();
            return new JsonResult(true);
        }

        [Route("api/[controller]/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteItem([FromBody] Guid id)
        {
            unitOfWork.ListItems.Delete(id);
            await unitOfWork.Save();
            return new JsonResult(true);
        }
    }
}
