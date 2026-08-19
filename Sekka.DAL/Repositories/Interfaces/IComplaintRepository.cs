using Sekka.DAL.Models;

namespace Sekka.DAL.Repositories.Interfaces
{
	public interface IComplaintRepository : IGenericRepository<Complaint, int>
	{
		Task<Complaint?> GetByTicketReferenceAsync(string ticketRef, CancellationToken ct = default);
		Task<IEnumerable<Complaint>> GetByComplainantAsync(int complainantId, CancellationToken ct = default);  // FIX: was string, must be int to match Complaint.ComplainantId
		Task<IEnumerable<Complaint>> GetByTripAsync(int tripId, CancellationToken ct = default);
		Task<IEnumerable<Complaint>> GetByStatusAsync(ComplaintStatus status, CancellationToken ct = default);
		Task<IEnumerable<Complaint>> GetByAgentAsync(int agentId, CancellationToken ct = default);
	}
}