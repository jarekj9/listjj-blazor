using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Listjj.Abstract;
using Listjj.Models;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Listjj.APIs
{

    [Route("api/[controller]/[action]")]
    [ApiController]

    public class ListjjController: ControllerBase
    {
        private readonly IListjjervice Listjjervice;
        private readonly ICategoryService CategoryService;
        private readonly IUserService UserService;

        public ListjjController(IListjjervice Listjjervice, IUserService userService, ICategoryService categoryService)
        {
            Listjjervice = Listjjervice;
            CategoryService = categoryService;
            UserService = userService;
        }
        public string GetValues()
        {
            return "Hello API";
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
                    return "Incorrect authorization header.";
                }
                throw;
            }
            var userId = UserService.FindUserIdByApiKey(Guid.Parse(key));
            if( userId == new Guid())
            {
                return "Unauthorized access.";
            }
            var items = await Listjjervice.GetItemsByUserId(userId.ToString());
            var response = System.Text.Json.JsonSerializer.Serialize(items);
            return response;
        }
        [HttpPost]
        public async Task<string> AddItem(string name="", string description="", string categoryName="default", string value="0")  
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
                    return "Incorrect authorization header.";
                }
                throw;
            }
            var userId = UserService.FindUserIdByApiKey(Guid.Parse(key));
            if( userId == new Guid())
            {
                return "Unauthorized access.";
            }

            using (var streamReader = new HttpRequestStreamReader(Request.Body, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jsonData = await JObject.LoadAsync(jsonReader);
                name = jsonData.GetValue("name")?.ToString() ?? "";
                description = jsonData.GetValue("description")?.ToString() ?? "";
                value = jsonData.GetValue("value")?.ToString() ?? "0";
                categoryName = jsonData.GetValue("categoryName")?.ToString() ?? "default";
            }

            var category = await CategoryService.FindByName(categoryName);
            if(category == null)
            {
                return "{\"status\":\"category not found\"}";
            }
            var item = new ListItem();
            item.UserId = userId.ToString();
            item.Created = DateTime.Now;
            item.Modified = DateTime.Now;
            item.CategoryId = category.Id;
            item.Name = name;
            item.Description = description;
            Double.TryParse(value, out double parsedValue);
            item.Value = parsedValue;
            await Listjjervice.AddListItem(item);
            return "{\"status\":\"ok\"}";
        }

    }
}