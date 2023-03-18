using Microsoft.EntityFrameworkCore;
using Listjj_frontend.Data;
using Listjj_frontend.Models;
using Listjj_frontend.Services.Abstract;

namespace Listjj_frontend.Repository

{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
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
