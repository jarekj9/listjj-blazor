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
using System.Security.Claims;
using Listjj.Infrastructure.Enums;
using Ganss.Xss;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class ItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ItemController> _logger;
        private readonly IMapper _apper;
        private readonly HtmlSanitizer _htmlSanitizer;

        public ItemController(IUnitOfWork unitOfWork, ILogger<ItemController> logger, IMapper mapper, HtmlSanitizer htmlSanitizer)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _apper = mapper;
            _htmlSanitizer = htmlSanitizer;
        }

        [Route("api/item/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllItems()
        {
            var items = await _unitOfWork.ListItems.GetAll();
            var itemsVms = _apper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Route("api/item/items_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByUserId()
        {
            var userId = GetUserId();
            var items = await _unitOfWork.ListItems.GetAllByUserId(userId);
            var itemsVms = _apper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Route("api/item/get_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetById(string id)
        {
            var itemIdGuid = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var item = await _unitOfWork.ListItems.GetByIdWithFiles(itemIdGuid);
            var itemVm = _apper.Map<ListItemViewModel>(item);
            return new JsonResult(itemVm);
        }

        [Route("api/item/items_by_categoryid")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByCategoryId(string categoryId)
        {
            var categoryIdGuid = Guid.TryParse(categoryId, out var guid) ? guid : Guid.Empty;
            var items = await _unitOfWork.ListItems.GetAllByCategoryId(categoryIdGuid);
            var itemsVms = _apper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }

        [Route("api/item/items_by_filter")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByFilter(string searchWords, string fromDateStr, string toDateStr, string categoryId)
        {
            searchWords = HttpUtility.UrlDecode(searchWords);
            var fromDate =   DateTime.TryParse(HttpUtility.UrlDecode(fromDateStr), out var parsedFrom) ? parsedFrom : DateTime.MinValue;
            var toDate = DateTime.TryParse(HttpUtility.UrlDecode(toDateStr), out var parsedTo) ? parsedTo : DateTime.MaxValue;
            var categoryIdGuid = Guid.TryParse(categoryId, out var parsedCategoryId) ? parsedCategoryId : Guid.Empty;
            var filter = MakeSearchFilter(categoryIdGuid, fromDate, toDate, searchWords);
            var items = await _unitOfWork.ListItems.ExecuteQuery(filter);
            var itemsVms = _apper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
        }
        private Expression<Func<ListItem, bool>> MakeSearchFilter(Guid categoryId, DateTime fromDate, DateTime toDate, string searchText)
        {
            var userId = GetUserId();
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

        [Route("api/item/addorupdate")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateItem([FromBody] ListItemViewModel itemVm)
        {
            itemVm.Name = _htmlSanitizer.Sanitize(itemVm.Name);
            itemVm.Description = _htmlSanitizer.Sanitize(itemVm.Description);
            itemVm.Tags = itemVm.Tags?.Select(tag => _htmlSanitizer.Sanitize(tag)).ToList();

            var existingItem = await _unitOfWork.ListItems.GetById(itemVm.Id);
            itemVm.UserId = GetUserId();
            if (itemVm.UserId == Guid.Empty)
            {
                return new JsonResult(false);
            }

            if (existingItem != null)
            {
                _apper.Map<ListItemViewModel, ListItem>(itemVm, existingItem);
                existingItem.Modified = DateTime.UtcNow;
                _unitOfWork.ListItems.Update(existingItem);
            }
            else
            {
                var newItem = _apper.Map<ListItem>(itemVm);
                newItem.Modified = DateTime.UtcNow;
                var allSequenceNumbers = (await _unitOfWork.ListItems.GetAllByCategoryId(itemVm.CategoryId)).Select(i => i.SequenceNumber).ToList();
                newItem.SequenceNumber = allSequenceNumbers.Count > 0 ? allSequenceNumbers.Max() + 1 : 1;
                _unitOfWork.ListItems.Add(newItem);
            }

            await _unitOfWork.Save();
            return new JsonResult(true);
        }

        [Route("api/item/sort_by_tags")]
        [HttpPost]
        public async Task<JsonResult> SortByTags([FromBody] string categoryId)
        {
            var categoryIdGuid = Guid.TryParse(categoryId, out var guid) ? guid : Guid.Empty;
            var items = await _unitOfWork.ListItems.GetAllByCategoryId(categoryIdGuid);
            items = items.OrderBy(i => i.Tags).ToList();
            var nextSequenceNumber = 0;
            foreach (var item in items)
            {
                item.SequenceNumber = nextSequenceNumber;
                nextSequenceNumber++;
                _unitOfWork.ListItems.Update(item);
            }
            await _unitOfWork.Save();
            return new JsonResult(true);
        }

        [Route("api/item/move")]
        [HttpPost]
        public async Task<JsonResult> Move([FromBody] Guid id, [FromQuery] MoveDirection direction)
        {
            var movedItem = await _unitOfWork.ListItems.GetById(id);
            var movedItemSequence = movedItem?.SequenceNumber ?? 0;

            if (direction == MoveDirection.Up && movedItem != null)
            {
                var previousItem = (await _unitOfWork.ListItems.GetAllByCategoryId(movedItem.CategoryId))
                    .Where(i => i.SequenceNumber < movedItemSequence)
                    .OrderBy(i => i.SequenceNumber).LastOrDefault();
                var previousItemSequence = previousItem?.SequenceNumber ?? -1;
                if (previousItemSequence == -1)
                {
                    return new JsonResult(false);
                }
                movedItem.SequenceNumber = previousItemSequence;
                previousItem.SequenceNumber = movedItemSequence;
                _unitOfWork.ListItems.Update(movedItem);
                _unitOfWork.ListItems.Update(previousItem);
            }

            if (direction == MoveDirection.Down && movedItem != null)
            {
                var nextItem = (await _unitOfWork.ListItems.GetAllByCategoryId(movedItem.CategoryId))
                    .Where(i => i.SequenceNumber > movedItemSequence)
                    .OrderBy(i => i.SequenceNumber).FirstOrDefault();
                var nextItemSequence = nextItem?.SequenceNumber ?? -1;
                if (nextItemSequence == -1)
                {
                    return new JsonResult(false);
                }
                movedItem.SequenceNumber = nextItemSequence;
                nextItem.SequenceNumber = movedItemSequence;
                _unitOfWork.ListItems.Update(movedItem);
                _unitOfWork.ListItems.Update(nextItem);
            }
            try
            {
                await _unitOfWork.Save();
                return new JsonResult(true);
            }
            catch (Exception ex)
            {
                return new JsonResult(false);
            }
        }


        [Route("api/item/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteItem([FromBody] Guid id)
        {
            _unitOfWork.ListItems.Delete(id);
            await _unitOfWork.Save();
            return new JsonResult(true);
        }
        private Guid GetUserId()
        {
            var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.TryParse(userIdStr, out var parsed) ? parsed : Guid.Empty;
            return userId;
        }
    }
}
