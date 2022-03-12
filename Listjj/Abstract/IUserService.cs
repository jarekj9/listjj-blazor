using System.Threading.Tasks;
using Listjj.Models;

namespace Listjj.Abstract
{
    public interface IUserService
    {
        string GetRole(ApplicationUser user);
    }
}
