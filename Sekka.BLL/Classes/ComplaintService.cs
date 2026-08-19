using AutoMapper;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL.Services
{
	public class ComplaintService : IComplaintService
	{
		private readonly IUnitOfWork _uow;
		private readonly IComplaintRepository _repo;
		private readonly IMapper _mapper;

		public ComplaintService(IUnitOfWork uow, IMapper mapper)
		{
			_uow = uow;
			_repo = uow.ComplaintRepository;
			_mapper = mapper;
		}


		public async Task<IEnumerable<ComplaintVM>> GetAllAsync()
		{
			var list = await _repo.GetAllAsync();
			return _mapper.Map<IEnumerable<ComplaintVM>>(list);
		}

		public async Task<ComplaintVM?> GetByIdAsync(int id)
		{
			var c = await _repo.GetByIdAsync(id);
			return c is null ? null : _mapper.Map<ComplaintVM>(c);
		}

		public async Task<ComplaintVM?> GetByTicketReferenceAsync(string ticketRef)
		{
			var c = await _repo.GetByTicketReferenceAsync(ticketRef);
			return c is null ? null : _mapper.Map<ComplaintVM>(c);
		}

		public async Task<IEnumerable<ComplaintVM>> GetByComplainantAsync(int complainantId)
		{
			var list = await _repo.GetByComplainantAsync(complainantId);
			return _mapper.Map<IEnumerable<ComplaintVM>>(list);
		}

		public async Task<IEnumerable<ComplaintVM>> GetByTripAsync(int tripId)
		{
			var list = await _repo.GetByTripAsync(tripId);
			return _mapper.Map<IEnumerable<ComplaintVM>>(list);
		}

		public async Task<IEnumerable<ComplaintVM>> GetByStatusAsync(ComplaintStatus status)
		{
			var list = await _repo.GetByStatusAsync(status);
			return _mapper.Map<IEnumerable<ComplaintVM>>(list);
		}

		public async Task<IEnumerable<ComplaintVM>> GetByAgentAsync(int agentId)
		{
			var list = await _repo.GetByAgentAsync(agentId);
			return _mapper.Map<IEnumerable<ComplaintVM>>(list);
		}


		public async Task<ComplaintVM> CreateAsync(ComplaintVM vm)
		{
			var model = _mapper.Map<Complaint>(vm);

			if (string.IsNullOrEmpty(model.TicketReference))
				model.TicketReference = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

			model.Status = ComplaintStatus.Open;
			model.CreatedAt = DateTime.UtcNow;

			_repo.AddAsync(model);
			await _uow.SaveChangesAsync();

			return _mapper.Map<ComplaintVM>(model);
		}

		public async Task<bool> UpdateAsync(ComplaintVM vm)
		{
			var existing = await _repo.GetByIdAsync(vm.Id);
			if (existing is null) return false;

			_mapper.Map(vm, existing);

			_repo.Update(existing);
			await _uow.SaveChangesAsync();
			return true;
		}

		public async Task<bool> AssignAgentAsync(int complaintId, int agentId)
		{
			var complaint = await _repo.GetByIdAsync(complaintId);
			if (complaint is null) return false;

			complaint.AssignedAgentId = agentId;

			if (complaint.Status == ComplaintStatus.Open)
				complaint.Status = ComplaintStatus.InReview;

			_repo.Update(complaint);
			await _uow.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateStatusAsync(int complaintId, ComplaintStatus newStatus)
		{
			var complaint = await _repo.GetByIdAsync(complaintId);
			if (complaint is null) return false;

			complaint.Status = newStatus;

			_repo.Update(complaint);
			await _uow.SaveChangesAsync();
			return true;
		}

		public async Task<bool> ResolveAsync(int complaintId, string resolutionNotes)
		{
			var complaint = await _repo.GetByIdAsync(complaintId);
			if (complaint is null) return false;

			complaint.Status = ComplaintStatus.Resolved;
			complaint.ResolutionNotes = resolutionNotes;
			complaint.ResolvedAt = DateTime.UtcNow;

			_repo.Update(complaint);
			await _uow.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var complaint = await _repo.GetByIdAsync(id);
			if (complaint is null) return false;

			_repo.Delete(complaint);
			await _uow.SaveChangesAsync();
			return true;
		}
	}
}