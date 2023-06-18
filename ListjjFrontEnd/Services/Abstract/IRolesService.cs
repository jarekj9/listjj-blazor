using Listjj.Infrastructure.ViewModels;

namespace ListjjFrontEnd.Services.Abstract
{
    public interface IRolesService
    {
        Task<List<RoleViewModel>> GetAllRoles();
    }
}
