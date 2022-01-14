using Listjj.Abstract;
using Listjj.Data;
using Listjj.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Listjj.Repository
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetByApiKey(Guid apiKey)
        {
            return await _context.Users.Where(x => x.ApiKey == apiKey).FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateApiKey(ApplicationUser user)
        {
            var newKey = Guid.NewGuid();
            user.ApiKey = newKey;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return newKey;
        }

    }
}
