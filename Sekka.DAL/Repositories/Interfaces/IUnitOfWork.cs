namespace Sekka.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T, TKey> GetRepo<T, TKey>() where T : class;

        Task<int> SaveChangesAsync();
        public IDriverRepository DriverRepository { get; }

    }
}
