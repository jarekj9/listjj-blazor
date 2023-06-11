using Listjj_frontend.Models;

namespace Listjj_frontend.Services.Abstract
{
    public interface IUserService
    {
        string GetRole(ApplicationUser user);
    }
}
