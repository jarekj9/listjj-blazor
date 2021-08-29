using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Listjj.Abstract;
using Listjj.Models;
using Listjj.Data;

namespace Listjj.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext  _appDbContext;

        public UserService (AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ApplicationUser FindUserByUserId(string userId)
        {
            return _appDbContext.Users.Where(x => x.Id == userId).FirstOrDefault();
        }
        public Guid CreateApiKey(ApplicationUser user)
        {
            var newKey = Guid.NewGuid();
            user.ApiKey = newKey;
            _appDbContext.Users.Update(user);
            _appDbContext.SaveChangesAsync();
            return newKey;
        }
        public Guid FindUserIdByApiKey(Guid apiKey)
        {
            var user = _appDbContext.Users.Where(x => x.ApiKey == apiKey).FirstOrDefault();
            Guid.TryParse(user?.Id, out var userId);
            return userId;
        }

        public Guid FindApiKeyByUserId(string userId)
        {
            return _appDbContext.Users.Where(x => x.Id == userId).FirstOrDefault().ApiKey;
        }
    }
}