using AutoMapper;
using Listjj.Abstract;
using Listjj.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Listjj.Models;
using Listjj.Pages;
using System.Collections.Generic;

namespace Listjj.APIs
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CategoryController> logger;
        private readonly IMapper mapper;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        [Route("api/[controller]/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllCategories()
        {
            var categories = await unitOfWork.Categories.GetAll();
            var categoriesVms = mapper.Map<List<CategoryViewModel>>(categories);
            return new JsonResult(categoriesVms);
        }

        [Route("api/[controller]/category_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetCategoryById(string id)
        {
            var categoryId = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var category = await unitOfWork.Categories.GetById(categoryId);
            var categoryVm = mapper.Map<CategoryViewModel>(category);
            return new JsonResult(categoryVm);
        }

        [Route("api/[controller]/categories_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetCategoriesByUserId(string userId)
        {
            var userIdGuid = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
            var categories = await unitOfWork.Categories.GetAllByUserId(userIdGuid);
            var categoriesVms = mapper.Map<List<CategoryViewModel>>(categories);
            return new JsonResult(categoriesVms);
        }

        [Route("api/[controller]/addorupdate")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateCategory([FromBody] CategoryViewModel categoryVm)
        {
            var existingCategory = await unitOfWork.Categories.GetById(categoryVm.Id);

            if(existingCategory != null)
            {
                mapper.Map<CategoryViewModel, Category>(categoryVm, existingCategory);
                unitOfWork.Categories.Update(existingCategory);
            }
            else
            {
                var newCategory = mapper.Map<Category>(categoryVm);
                unitOfWork.Categories.Add(newCategory);
            }

            await unitOfWork.Save();
            return new JsonResult(true);
        }

        [Route("api/[controller]/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteCategory([FromBody] Guid id)
        {
            unitOfWork.Categories.Delete(id);
            await unitOfWork.Save();
            return new JsonResult(true);
        }

    }
}
