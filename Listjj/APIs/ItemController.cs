using AutoMapper;
using Listjj.Abstract;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Listjj.Transaction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [Route("api/[controller]/items_by_categoryid")]
        [HttpGet]
        public async Task<JsonResult> GetItemsByCategoryId(string categoryId)
        {
            var categoryIdGuid = Guid.TryParse(categoryId, out var guid) ? guid : Guid.Empty;
            var items = await unitOfWork.ListItems.GetAllByCategoryId(categoryIdGuid);
            var itemsVms = mapper.Map<List<ListItemViewModel>>(items);
            return new JsonResult(itemsVms);
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
