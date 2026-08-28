namespace Sekka.BLL.Interfaces
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Sekka.BLL.ViewModels;
	using Sekka.DAL.Models;

	public interface IComplaintService
	{
		Task<IEnumerable<ComplaintVM>> GetAllAsync();
		Task<ComplaintVM?> GetByIdAsync(int id);
		Task<ComplaintVM?> GetByTicketReferenceAsync(string ticketRef);

		Task<IEnumerable<ComplaintVM>> GetByComplainantAsync(string complainantId);

		Task<IEnumerable<ComplaintVM>> GetByTripAsync(int tripId);
		Task<IEnumerable<ComplaintVM>> GetByStatusAsync(ComplaintStatus status);

		Task<IEnumerable<ComplaintVM>> GetByAgentAsync(string agentId);

		Task<ComplaintVM> CreateAsync(ComplaintVM vm);
		Task<bool> UpdateAsync(ComplaintVM vm);

		Task<bool> AssignAgentAsync(int complaintId, string agentId);

		Task<bool> UpdateStatusAsync(int complaintId, ComplaintStatus newStatus);
		Task<bool> ResolveAsync(int complaintId, string resolutionNotes);
		Task<bool> DeleteAsync(int id);
	}
}