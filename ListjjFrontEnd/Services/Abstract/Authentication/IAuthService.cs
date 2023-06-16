using Listjj.Infrastructure.Data;
using Listjj.Infrastructure.Models;

namespace ListjjFrontEnd.Services.Abstract.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResult> Register(RegisterModel registerModel);
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
