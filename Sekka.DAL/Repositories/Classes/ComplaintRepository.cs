using Microsoft.EntityFrameworkCore;
using Sekka.DAL.Context;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.DAL.Repositories.Classes
{
	public class ComplaintRepository : GenericRepository<Complaint, int>, IComplaintRepository
	{
		private readonly SekkaDbContext _dbContext;

		public ComplaintRepository(SekkaDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Complaint?> GetByTicketReferenceAsync(string ticketRef, CancellationToken ct = default)
			=> await _dbContext.Complaints
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.TicketReference == ticketRef, ct);

		public async Task<IEnumerable<Complaint>> GetByComplainantAsync(int complainantId, CancellationToken ct = default)
			=> await _dbContext.Complaints
				.AsNoTracking()
				.Where(c => c.ComplainantId == complainantId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync(ct);

		public async Task<IEnumerable<Complaint>> GetByTripAsync(int tripId, CancellationToken ct = default)
			=> await _dbContext.Complaints
				.AsNoTracking()
				.Where(c => c.TripId == tripId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync(ct);

		public async Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default)
			=> await _dbContext.Complaints
				.AsNoTracking()
				.Where(c => c.Status == status)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync(ct);

		public async Task<IEnumerable<Complaint>> GetByAgentAsync(int agentId, CancellationToken ct = default)
			=> await _dbContext.Complaints
				.AsNoTracking()
				.Where(c => c.AssignedAgentId == agentId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync(ct);
	}
}