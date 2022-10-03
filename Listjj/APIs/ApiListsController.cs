using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Listjj.Abstract;
using Listjj.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;


namespace Listjj.APIs
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ListjjController: ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly ILogger<ListjjController> logger;

        public ListjjController(IUnitOfWork unitOfWork, ILogger<ListjjController> _logger)
        {
            UnitOfWork = unitOfWork;
            logger = _logger;
        }
        public string GetValues()
        {
            return "{\"ping\":\"pong\"}";
        }

        private async Task<(bool success, string message, Guid userId)> IsAuthorized()
        {
            var headers = GetAllHeaders();
            string key;
            try
            {
                key = headers.Value["Authorization"].Split(' ')[1];
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is IndexOutOfRangeException)
                {
                    return (success:false, message:"Incorrect authorization header.", userId:Guid.Empty);
                }
                throw;
            }
            Guid.TryParse(key, out var parsedKey);
            var user =  await UnitOfWork.Users.GetByApiKey(parsedKey);
            //var userId = UserService.FindUserIdByApiKey(parsedKey);
            if (parsedKey == Guid.Empty || user.Id == new Guid())
            {
                return (success:false, message:"Unauthorized access.", userId: Guid.Empty);
            }
            return (success:true, message:"", userId: user.Id);
        }


        [HttpGet("GetAllHeaders")]
        public ActionResult<Dictionary<string, string>> GetAllHeaders()
        {
            Dictionary<string, string> requestHeaders =
                new Dictionary<string, string>();
            foreach (var header in Request.Headers)
            {
                requestHeaders.Add(header.Key, header.Value);
            }
            
            return requestHeaders;
        }

        public async Task<JsonResult> GetAllListjj()  
        {
            var isAuthorized = await IsAuthorized();
            if(!isAuthorized.success)
            {
                return new JsonResult(isAuthorized.message);
            }
            var items = await UnitOfWork.ListItems.GetAllByUserId(isAuthorized.userId);

            Response.Headers.Add("content-type", "application/json");
            return new JsonResult(items);
        }
        public async Task<JsonResult> GetByCategoryName(string name = "", string key = "")
        {
            Response.Headers.Add("content-type", "application/json");
            Guid.TryParse(key, out var parsedKey);
            var user = await UnitOfWork.Users.GetByApiKey(parsedKey);
            if (parsedKey == Guid.Empty || user.Id == new Guid())       // special auth for tizen watch
            {
                return new JsonResult(new Dictionary<string,string>() { { "status", "Unauthorized access." } });
            }

            // HTTP OPTIONS is necessary for preflight request, otherwise CORS will be a problem
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (Request.Method == "OPTIONS")
            {
                return new JsonResult("Options");
            }

            var categoryId = (await UnitOfWork.Categories.GetByName(name)).Id;
            var items = (await UnitOfWork.ListItems.GetAllByCategoryId(categoryId));
            if (items.Count == 0)
            {
                return new JsonResult(new Dictionary<string, string>() { { "status", "List is empty." } });
            }
            var response = System.Text.Json.JsonSerializer.Serialize(items[0]);

            
            return new JsonResult(items[0]);
        }

        [HttpPost]
        public async Task<JsonResult> AddItem(string name="", string description="", string categoryName="default", string value="0", string id = "")  
        {
            var isAuthorized = await IsAuthorized();
            if (!isAuthorized.success)
            {
                return new JsonResult(isAuthorized.message);
            }

            using (var streamReader = new HttpRequestStreamReader(Request.Body, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jsonData = await JObject.LoadAsync(jsonReader);
                name = jsonData.GetValue("name")?.ToString() ?? "";
                description = jsonData.GetValue("description")?.ToString() ?? "";
                value = jsonData.GetValue("value")?.ToString() ?? "0";
                id = jsonData.GetValue("id")?.ToString() ?? "";
                categoryName = jsonData.GetValue("categoryName")?.ToString() ?? "default";
            }

            Response.Headers.Add("content-type", "application/json");

            Double.TryParse(value, out double parsedValue);
            var category = await UnitOfWork.Categories.GetByName(categoryName);
            if (category == null)
            {
                return new JsonResult(new Dictionary<string, string>() { { "status", "Category not found." } });
            }

            Guid.TryParse(id, out var parsedId);
            var item = (await UnitOfWork.ListItems.GetById(parsedId)) ?? new ListItem();
            item.CategoryId = category.Id;
            item.Name = name;
            item.Description = description;
            item.Value = parsedValue;

            if (item.Id != Guid.Empty)
            {
                UnitOfWork.ListItems.Update(item);
                await UnitOfWork.Save();
                return new JsonResult(new Dictionary<string, string>() { { "status", "ok" } });
            }

            item.Id = parsedId;
            item.UserId = isAuthorized.userId;
            item.Created = DateTime.Now;
            item.Modified = DateTime.Now;
            UnitOfWork.ListItems.Add(item);
            await UnitOfWork.Save();
            return new JsonResult(new Dictionary<string, string>() { { "status", "ok" } });
        }

        [HttpOptions]
        [HttpPost]
        public async Task<JsonResult> DelItem(string id)
        {
            // HTTP OPTIONS is necessary for preflight request, otherwise CORS will be a problem
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin, Accept, Authorization, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
            if (Request.Method == "OPTIONS")
            {
                Console.WriteLine("####### Sending Options");
                return new JsonResult("Options");
            }

            var isAuthorized = await IsAuthorized();
            if (!isAuthorized.success)
            {
                return new JsonResult(isAuthorized.message);
            }

            using (var streamReader = new HttpRequestStreamReader(Request.Body, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jsonData = await JObject.LoadAsync(jsonReader);
                id = jsonData.GetValue("id")?.ToString() ?? "";
            }

            Guid.TryParse(id, out var idGuid);
            if (idGuid == new Guid())
            {
                Console.WriteLine("####### Sending Incorrect id.");
                return new JsonResult("Incorrect id.");
            }
            var item = await UnitOfWork.ListItems.GetById(idGuid);
            if (item == null)
            {
                Console.WriteLine("####### Sending Item not found");
                return new JsonResult("Item not found.");
            }
            UnitOfWork.ListItems.Delete(item.Id);
            await UnitOfWork.Save();

            Console.WriteLine("####### Sending ok");
            return new JsonResult("Deleted.");
        }

    }
}