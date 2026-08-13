using Sekka.DAL.Models;

namespace Sekka.DAL.Repositories.Interfaces
{
    public interface IDriverRepository : IGenericRepository<Driver, int>
    {
        Task<IEnumerable<Driver>> GetAllWithUserAsync(CancellationToken ct = default);
        Task<Driver?> GetDriverDetailsByIdAsync(int id, CancellationToken ct = default);
    }
}
