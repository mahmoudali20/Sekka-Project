using Microsoft.EntityFrameworkCore;
using Sekka.DAL.Context;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.DAL.Repositories.Classes
{
    public class DriverRepository : GenericRepository<Driver, int>, IDriverRepository
    {
        private readonly SekkaDbContext _context;

        public DriverRepository(SekkaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllWithUserAsync(CancellationToken ct = default)
        {
            var query = _context.Drivers.Include(d => d.User);

            return await query.ToListAsync(ct);
        }


        public async Task<Driver?> GetDriverDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Drivers.Include(d => d.User).Include(d => d.Car).FirstOrDefaultAsync(d => d.Id == id, ct);
        }
    }
}
