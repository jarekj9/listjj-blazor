using AutoMapper;
using Listjj.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Listjj.Models;

namespace Listjj.APIs
{
    [Authorize(Roles = "Admin,User")]
    public class ExternalApiKeyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<ExternalApiKeyController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;


        public ExternalApiKeyController(IUnitOfWork unitOfWork, ILogger<ExternalApiKeyController> logger,
            UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [Route("api/externalapikey/get")]
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var userId = GetUserId();
            var user = await userManager.FindByIdAsync(userId.ToString());
            return new JsonResult(user.ApiKey);
        }

        [Route("api/externalapikey/generate")]
        [HttpGet]
        public async Task<JsonResult> Generate()
        {
            var userId = GetUserId();
            var user = await userManager.FindByIdAsync(userId.ToString());
            var apiKey = await unitOfWork.Users.CreateApiKey(user);
            return new JsonResult(apiKey);
        }

        [Route("api/externalapikey/set")]
        [HttpPost]
        public async Task<JsonResult> SetApiKey([FromBody] string apikey)
        {
            var userId = GetUserId();
            var user = await userManager.FindByIdAsync(userId.ToString());
            var apiKey = await unitOfWork.Users.SetApiKey(user, apikey);
            return new JsonResult(apiKey);
        }

        private Guid GetUserId()
        {
            var userIdStr = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.TryParse(userIdStr, out var parsed) ? parsed : Guid.Empty;
            return userId;
        }
    }
}
