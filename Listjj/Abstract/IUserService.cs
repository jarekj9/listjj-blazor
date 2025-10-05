using Listjj.Models;
using System.Threading.Tasks;

namespace Listjj.Abstract
{
    public interface IUserService
    {
        string GetRole(ApplicationUser user);
        Task<ApplicationUser> GetByApiKey(string apiKey);
    }
}
