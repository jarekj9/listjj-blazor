using Listjj.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Ganss.Xss;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class TagsController : Controller
    {
        private readonly ILogger<TagsController> _logger;
        private readonly ITagsCacheService _tagsCacheService;
        private readonly HtmlSanitizer _htmlSanitizer;

        public TagsController(ILogger<TagsController> logger, ITagsCacheService tagsCacheService, HtmlSanitizer htmlSanitizer)
        {
            this._logger = logger;
            this._tagsCacheService = tagsCacheService;
            _htmlSanitizer = htmlSanitizer;
        }

        [Route("api/[controller]/get_by_userid")]
        [HttpGet]
        public async Task<JsonResult> GetByUserId()
        {
            var userId = GetUserId();
            var usersTags = await _tagsCacheService.GetTagsSelectionAsync(userId);
            return new JsonResult(usersTags);
        }

        [Route("api/[controller]/update")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateCategory([FromBody] List<string> tags)
        {
            var sanitizedTags = tags?.ConvertAll(tag => _htmlSanitizer.Sanitize(tag));
            var userId = GetUserId();
            await _tagsCacheService.UpdateCache(userId, sanitizedTags);
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
