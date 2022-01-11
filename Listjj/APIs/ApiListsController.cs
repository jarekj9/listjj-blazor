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
        private readonly IListItemRepository ListItemRepository;
        private readonly ICategoryRepository CategoryRepository;
        private readonly IUserService UserService;
        private readonly ILogger<ListjjController> logger;

        public ListjjController(IListItemRepository listItemRepository, IUserService userService, ICategoryRepository categoryRepository, ILogger<ListjjController> _logger)
        {
            ListItemRepository = listItemRepository;
            CategoryRepository = categoryRepository;
            UserService = userService;
            logger = _logger;
        }
        public string GetValues()
        {
            return "Hello API";
        }

        private (bool success, string message, Guid userId) IsAuthorized()
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
            var userId = UserService.FindUserIdByApiKey(parsedKey);
            if (parsedKey == Guid.Empty || userId == new Guid())
            {
                return (success:false, message:"Unauthorized access.", userId: Guid.Empty);
            }
            return (success:true, message:"", userId: userId);
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

        public async Task<string> GetAllListjj()  
        {
            var isAuthorized = IsAuthorized();
            if(!isAuthorized.success)
            {
                return isAuthorized.message;
            }
            var items = await ListItemRepository.GetAllByUserId(isAuthorized.userId.ToString());
            //var items = await ListItemService.GetItemsByUserId(isAuthorized.userId.ToString());

            var response = System.Text.Json.JsonSerializer.Serialize(items);

            return response;
        }
        public async Task<string> GetByCategoryName(string name = "", string key = "")
        {
            Guid.TryParse(key, out var parsedKey);
            var userId = UserService.FindUserIdByApiKey(parsedKey);
            if (parsedKey == Guid.Empty || userId == new Guid())       // special auth for tizen watch
            {
                return "{\"status\":\"Unauthorized access.\"}";
            }

            // HTTP OPTIONS is necessary for preflight request, otherwise CORS will be a problem
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            if (Request.Method == "OPTIONS")
            {
                return "Options";
            }

            var categoryId = (await CategoryRepository.GetByName(name)).Id;
            //var categoryId = (await CategoryService.FindByName(name)).Id;
            var items = (await ListItemRepository.GetAllByCategoryId(categoryId));
            //var items = (await ListItemService.GetItemsByCategoryId(categoryId));
            if (items.Count == 0)
            {
                return "{\"status\":\"List is empty.\"}";
            }
            var response = System.Text.Json.JsonSerializer.Serialize(items[0]);

            return response;
        }

        [HttpPost]
        public async Task<string> AddItem(string name="", string description="", string categoryName="default", string value="0", string id = "")  
        {
            var isAuthorized = IsAuthorized();
            if (!isAuthorized.success)
            {
                return isAuthorized.message;
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

            Double.TryParse(value, out double parsedValue);
            var category = await CategoryRepository.GetByName(categoryName);
            //var category = await CategoryService.FindByName(categoryName);
            if (category == null)
            {
                return "{\"status\":\"category not found\"}";
            }

            Guid.TryParse(id, out var parsedId);
            var item = (await ListItemRepository.GetById(parsedId)) ?? new ListItem();
            //var item = (await ListItemService.FindById(parsedId)) ?? new ListItem();
            item.CategoryId = category.Id;
            item.Name = name;
            item.Description = description;
            item.Value = parsedValue;

            if (item.Id != Guid.Empty)
            {
                ListItemRepository.Update(item);
                await ListItemRepository.Save();
                //await ListItemService.UpdateListItem(item);
                return "{\"status\":\"ok\"}";
            }

            item.Id = parsedId;
            item.UserId = isAuthorized.userId.ToString();
            item.Created = DateTime.Now;
            item.Modified = DateTime.Now;
            ListItemRepository.Add(item);
            await ListItemRepository.Save();
            //await ListItemService.AddListItem(item);
            return "{\"status\":\"ok\"}";
        }

        [HttpOptions]
        [HttpPost]
        public async Task<string> DelItem(string id)
        {
            // HTTP OPTIONS is necessary for preflight request, otherwise CORS will be a problem
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin, Accept, Authorization, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
            if (Request.Method == "OPTIONS")
            {
                Console.WriteLine("####### Sending Options");
                return "Options";
            }

            var isAuthorized = IsAuthorized();
            if (!isAuthorized.success)
            {
                return isAuthorized.message;
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
                return "Incorrect id.";
            }
            var item = await ListItemRepository.GetById(idGuid);
            //var item = await ListItemService.FindById(idGuid);
            if (item == null)
            {
                Console.WriteLine("####### Sending Item not found");
                return "Item not found.";
            }
            ListItemRepository.Delete(item);
            await ListItemRepository.Save();
            //await ListItemService.DelListItem(item);

            Console.WriteLine("####### Sending ok");
            return "Deleted.";
        }

    }
}