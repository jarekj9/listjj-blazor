using Listjj.Infrastructure.Data;
using Listjj.Infrastructure.Models;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Listjj.APIs
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private static UserViewModel LoggedOutUser = new UserViewModel { IsAuthenticated = false };
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [Route("api/[controller]/register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(newUser, model.Password);

            if(newUser.Email == "admin@Listjj")
            {
                await _userManager.AddToRoleAsync(newUser, "Admin");
            }

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return Ok(new RegisterResult { Successful = false, Errors = errors });

            }

            return Ok(new RegisterResult { Successful = true });
        }
    }
}
