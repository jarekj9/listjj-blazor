using Listjj.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Listjj.APIs
{
    [Authorize]
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
        public async Task<JsonResult> GetByUserId(string id)
        {
            var userIdGuid = Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            var usersTags = await tagsCacheService.GetTagsSelectionAsync(userIdGuid);
            return new JsonResult(usersTags);
        }

        [Route("api/[controller]/{userId}/update")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateCategory(string userId, [FromBody] List<string> tags)
        {
            var userIdGuid = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
            await tagsCacheService.UpdateCache(userIdGuid, tags);
            return new JsonResult(true);
        }
    }
}
