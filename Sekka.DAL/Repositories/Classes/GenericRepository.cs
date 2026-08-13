using Microsoft.EntityFrameworkCore;
using Sekka.DAL.Context;
using Sekka.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Sekka.DAL.Repositories.Classes
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        private readonly SekkaDbContext _dbContext;

        public GenericRepository(SekkaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<T> query = tracking ? _dbContext.Set<T>() : _dbContext.Set<T>().AsNoTracking();

            return await query.ToListAsync(ct);
        }

        public async Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return await _dbContext.Set<T>().FindAsync(new object?[] { id }, ct);
        }

        public void AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            return await _dbContext.Set<T>().AsNoTracking().AnyAsync(predicate, ct);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<T> query = tracking ? _dbContext.Set<T>() : _dbContext.Set<T>().AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate, ct);
        }
    }
}