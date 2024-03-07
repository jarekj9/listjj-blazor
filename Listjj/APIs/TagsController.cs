using Listjj.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class TagsController : Controller
    {
        private readonly ILogger<TagsController> logger;
        private readonly ITagsCacheService tagsCacheService;

        public TagsController(ILogger<TagsController> logger, ITagsCacheService tagsCacheService)
        {
            this.logger = logger;
            this.tagsCacheService = tagsCacheService;
        }

        [Route("api/[controller]/get_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetByUserId()
        {
            var userId = GetUserId();
            var usersTags = await tagsCacheService.GetTagsSelectionAsync(userId);
            return new JsonResult(usersTags);
        }

        [Route("api/[controller]/update")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateCategory([FromBody] List<string> tags)
        {
            var userId = GetUserId();
            await tagsCacheService.UpdateCache(userId, tags);
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
