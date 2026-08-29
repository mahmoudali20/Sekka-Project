using Sekka.DAL.Models;

namespace Sekka.DAL.Repositories.Interfaces
{
	public interface IComplaintRepository : IGenericRepository<Complaint, int>
	{
		Task<Complaint?> GetByTicketReferenceAsync(string ticketRef, CancellationToken ct = default);

		Task<IEnumerable<Complaint>> GetByComplainantAsync(string complainantId, CancellationToken ct = default);

		Task<IEnumerable<Complaint>> GetByTripAsync(int tripId, CancellationToken ct = default);
		Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default);

		Task<IEnumerable<Complaint>> GetByAgentAsync(string agentId, CancellationToken ct = default);
	}
}