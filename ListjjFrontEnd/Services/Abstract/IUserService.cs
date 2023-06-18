using Listjj.Infrastructure.ViewModels;

namespace ListjjFrontEnd.Services.Abstract
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetAllUsers();
        Task<bool> AddorUpdate(UserViewModel user);
        Task<bool> Delete(UserViewModel user);
    }
}
