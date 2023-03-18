using Listjj_frontend.Models;

namespace Listjj_frontend.Services.Abstract
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByApiKey(Guid apiKey);
        Task<Guid> CreateApiKey(ApplicationUser user);
    }
}