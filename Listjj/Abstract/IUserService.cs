using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Listjj.Models;

namespace Listjj.Abstract
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> FindAllUsers();
        ApplicationUser FindUserByUserId(string userId);
        Guid FindUserIdByApiKey(Guid apiKey);
        Guid CreateApiKey(ApplicationUser user);
        Guid FindApiKeyByUserId(string userId);
    }
}