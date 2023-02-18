using Listjj.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Listjj.APIs
{
    public class ItemController : Controller
    {
        private readonly IUnitOfWork UnitOfWork;
        private readonly ILogger<ItemController> logger;

        public ItemController(IUnitOfWork unitOfWork, ILogger<ItemController> logger)
        {
            UnitOfWork = unitOfWork;
            logger = logger;
        }

        [Route("api/[controller]/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllItems()
        {
            var items = await UnitOfWork.ListItems.GetAll();

            return new JsonResult(items);
        }
    }
}
