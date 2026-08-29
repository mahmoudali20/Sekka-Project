using Sekka.DAL.Context;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.DAL.Repositories.Classes
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly SekkaDbContext _dbContext;
		private readonly Dictionary<Type, object> _repositories = [];

		public UnitOfWork(SekkaDbContext dbContext, IDriverRepository driverRepository, IComplaintRepository complaintRepository)
		{
			_dbContext = dbContext;
			DriverRepository = driverRepository;
			ComplaintRepository = complaintRepository;
		}

		public IDriverRepository DriverRepository { get; }

		public IComplaintRepository ComplaintRepository { get; }

		public IGenericRepository<T, TKey> GetRepo<T, TKey>() where T : class
		{
			var type = typeof(T);

			if (_repositories.TryGetValue(type, out object? value))
				return (IGenericRepository<T, TKey>)value;

			var repo = new GenericRepository<T, TKey>(_dbContext);
			_repositories[type] = repo;
			return repo;
		}

		public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
	}
}