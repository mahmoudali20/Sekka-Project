using System.Linq.Expressions;

namespace Sekka.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default);

        void AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false, CancellationToken ct = default);
        Task<double> AverageAsync(Expression<Func<T, int>> selector,Expression<Func<T, bool>>? predicate = null,CancellationToken ct = default);
    }
}