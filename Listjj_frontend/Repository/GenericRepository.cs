using Listjj_frontend.Data;
using Listjj_frontend.Services.Abstract;
using Microsoft.EntityFrameworkCore;


namespace Listjj_frontend.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> table = null;

        public GenericRepository(ApplicationDbContext context)
        {
            this._context = context;
            table = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await table.FindAsync(id);
        }

        public void Add(T obj)
        {
            table.Add(obj);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
