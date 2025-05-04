using AutoMapper;
using Listjj.Abstract;
using Listjj.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Listjj.Models;
using System.Collections.Generic;
using Listjj.Infrastructure.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Ganss.Xss;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper mapper;
        private readonly ICategoryCacheService _categoryCacheService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HtmlSanitizer _htmlSanitizer;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger, IMapper mapper, ICategoryCacheService categoryCacheService, UserManager<ApplicationUser> userManager, HtmlSanitizer htmlSanitizer)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this.mapper = mapper;
            this._categoryCacheService = categoryCacheService;
            this._userManager = userManager;
            _htmlSanitizer = htmlSanitizer;
        }

        [Route("api/category/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetAll();
            var categoriesVms = mapper.Map<List<CategoryViewModel>>(categories);
            return new JsonResult(categoriesVms);
        }

        [Route("api/category/category_by_id")]
        [HttpGet]
        public async Task<JsonResult> GetCategoryById(string id)
        {
            var categoryId = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var category = await _unitOfWork.Categories.GetById(categoryId);
            var categoryVm = mapper.Map<CategoryViewModel>(category);
            return new JsonResult(categoryVm);
        }

        [Route("api/category/categories_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetCategoriesByUserId()
        {
            var userIdGuid = GetUserId();
            var categories = await _unitOfWork.Categories.GetAllByUserId(userIdGuid);
            var categoriesVms = mapper.Map<List<CategoryViewModel>>(categories);
            return new JsonResult(categoriesVms);
        }

        [Route("api/category/recent_categoryid_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetRecentCategoryIdByUserId()
        {
            var userIdGuid = GetUserId();
            var recentCategoryId = await _categoryCacheService.GetRecentCategoryAsync(userIdGuid);
            return new JsonResult(recentCategoryId);
        }

        [Route("api/category/update_recent_category")]
        [HttpPost]
        public async Task<JsonResult> GetRecentCategoryIdByUserId([FromBody]UpdateCategoryRequest updateCategoryRequest)
        {
            var userIdGuid = GetUserId();
            await _categoryCacheService.UpdateRecentCategoryCache(userIdGuid, updateCategoryRequest.RecentCategoryId);
            return new JsonResult(true);
        }

        [Route("api/category/addorupdate")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateCategory([FromBody] CategoryViewModel categoryVm)
        {
            categoryVm.Name = _htmlSanitizer.Sanitize(categoryVm.Name);
            categoryVm.Description = _htmlSanitizer.Sanitize(categoryVm.Description);

            var existingCategory = await _unitOfWork.Categories.GetById(categoryVm.Id);
            categoryVm.UserId = GetUserId();
            if (categoryVm.UserId == Guid.Empty)
            {
                return new JsonResult(false);
            }
            if (existingCategory != null)
            {
                mapper.Map<CategoryViewModel, Category>(categoryVm, existingCategory);
                _unitOfWork.Categories.Update(existingCategory);
            }
            else
            {
                var newCategory = mapper.Map<Category>(categoryVm);
                _unitOfWork.Categories.Add(newCategory);
            }

            await _unitOfWork.Save();
            return new JsonResult(true);
        }

        [Route("api/category/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteCategory([FromBody] Guid id)
        {
            _unitOfWork.Categories.Delete(id);
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
