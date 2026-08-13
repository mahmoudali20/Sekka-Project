using Sekka.DAL.Context;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly SekkaDbContext _dbContext;

        private readonly Dictionary<Type, object> _repositories = []; // repositories Cache  
        public UnitOfWork(SekkaDbContext dbcontext, IDriverRepository driverRepository)
        {
            _dbContext = dbcontext;
            DriverRepository = driverRepository;
        }
        public IDriverRepository DriverRepository { get; }


        public IGenericRepository<T, TKey> GetRepo<T, TKey>() where T : class
        {

            //Check if repository already exists in the dictionary or not  ?? IDctionary will create it above
            // Like IGenericRepository<Member>   => need Name
            var type = typeof(T);
            // if exist in dictionary => use it 

            if (_repositories.TryGetValue(type, out object? value))
            {
                return (IGenericRepository<T, TKey>)value;
            }

            // if not exist => create repo => Add dictionary => retuen repo
            else
            {
                var repo = new GenericRepository<T, TKey>(_dbContext);
                _repositories[type] = repo;
                return repo;
            }
        }

        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
