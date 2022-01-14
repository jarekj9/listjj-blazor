using System.Threading.Tasks;
using System.Collections.Generic;
using Listjj.Models;
using System;

namespace Listjj.Abstract
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByApiKey(Guid apiKey);
        Task<Guid> CreateApiKey(ApplicationUser user);
    }
}