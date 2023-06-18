using AutoMapper;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Listjj.APIs
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly ILogger<RolesController> logger;
        private readonly IMapper mapper;
        private readonly RoleManager<ApplicationRole> roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager, ILogger<RolesController> logger, IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.roleManager = roleManager;
        }

        [Route("api/[controller]/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllRoles()
        {
            var roles = roleManager.Roles.ToList();
            var roleVms = mapper.Map<List<RoleViewModel>>(roles);
            return Json(roleVms);
        }
    }
}
