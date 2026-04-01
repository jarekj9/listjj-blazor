using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Listjj.APIs
{
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VersionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("api/version")]
        [HttpGet]
        public async Task<IActionResult> GetVersion()
        {
            var version = new
            {
                version = _configuration.GetValue<string>("Version"),
            };
            return Ok(version);
        }
    }
}
