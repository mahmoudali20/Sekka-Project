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

		private async Task HydrateNamesAsync(IEnumerable<ComplaintVM> vms)
		{
			var vmList = vms.ToList();
			if (!vmList.Any()) return;

			var userIds = vmList
				.SelectMany(v => new[] { v.ComplainantId, v.AssignedAgentId })
				.Where(id => !string.IsNullOrEmpty(id))
				.Distinct()
				.ToHashSet();

			var users = (await _uow.GetRepo<ApplicationUser, string>().GetAllAsync(tracking: false))
						?.Where(u => userIds.Contains(u.Id))
						.ToDictionary(u => u.Id)
						?? new Dictionary<string, ApplicationUser>();

			var tripIds = vmList
				.Where(v => v.TripId.HasValue)
				.Select(v => v.TripId!.Value)
				.Distinct()
				.ToHashSet();

			var driverNameByTripId = new Dictionary<int, string>();
			if (tripIds.Any())
			{
				var rides = (await _uow.GetRepo<Ride, int>().GetAllAsync(tracking: false))
							?.Where(r => tripIds.Contains(r.RideID) && r.DriverId.HasValue)
							.ToList()
							?? new List<Ride>();

				var driverIds = rides
					.Select(r => r.DriverId!.Value)
					.Distinct()
					.ToHashSet();

				var drivers = (await _uow.DriverRepository.GetAllWithUserAsync())
							  ?.Where(d => driverIds.Contains(d.Id))
							  .ToDictionary(d => d.Id)
							  ?? new Dictionary<int, Driver>();

				foreach (var ride in rides)
				{
					if (ride.DriverId.HasValue
						&& drivers.TryGetValue(ride.DriverId.Value, out var drv)
						&& drv.User?.FullName is string name)
					{
						driverNameByTripId[ride.RideID] = name;
					}
				}
			}

			foreach (var vm in vmList)
			{
				if (!string.IsNullOrEmpty(vm.ComplainantId)
					&& users.TryGetValue(vm.ComplainantId, out var complainant))
					vm.ComplainantName = complainant.FullName;

				if (!string.IsNullOrEmpty(vm.AssignedAgentId)
					&& users.TryGetValue(vm.AssignedAgentId, out var agent))
					vm.AssignedAgentName = agent.FullName;

				if (vm.TripId.HasValue
					&& driverNameByTripId.TryGetValue(vm.TripId.Value, out var driverName))
					vm.DriverName = driverName;
			}
		}

		private async Task HydrateNamesAsync(ComplaintVM vm)
			=> await HydrateNamesAsync(new[] { vm });


		public async Task<IEnumerable<ComplaintVM>> GetAllAsync()
		{
			var list = await _repo.GetAllAsync();
			var vms = _mapper.Map<IEnumerable<ComplaintVM>>(list);
			await HydrateNamesAsync(vms);
			return vms;
		}

		public async Task<ComplaintVM?> GetByIdAsync(int id)
		{
			var c = await _repo.GetByIdAsync(id);
			if (c is null) return null;
			var vm = _mapper.Map<ComplaintVM>(c);
			await HydrateNamesAsync(vm);
			return vm;
		}

		public async Task<ComplaintVM?> GetByTicketReferenceAsync(string ticketRef)
		{
			var c = await _repo.GetByTicketReferenceAsync(ticketRef);
			if (c is null) return null;
			var vm = _mapper.Map<ComplaintVM>(c);
			await HydrateNamesAsync(vm);
			return vm;
		}

		public async Task<IEnumerable<ComplaintVM>> GetByComplainantAsync(string complainantId)
		{
			var list = await _repo.GetByComplainantAsync(complainantId);
			var vms = _mapper.Map<IEnumerable<ComplaintVM>>(list);
			await HydrateNamesAsync(vms);
			return vms;
		}

		public async Task<IEnumerable<ComplaintVM>> GetByTripAsync(int tripId)
		{
			var list = await _repo.GetByTripAsync(tripId);
			var vms = _mapper.Map<IEnumerable<ComplaintVM>>(list);
			await HydrateNamesAsync(vms);
			return vms;
		}

		public async Task<IEnumerable<ComplaintVM>> GetByStatusAsync(ComplaintStatus status)
		{
			var list = await _repo.GetByStatusAsync(status);
			var vms = _mapper.Map<IEnumerable<ComplaintVM>>(list);
			await HydrateNamesAsync(vms);
			return vms;
		}

		public async Task<IEnumerable<ComplaintVM>> GetByAgentAsync(string agentId)
		{
			var list = await _repo.GetByAgentAsync(agentId);
			var vms = _mapper.Map<IEnumerable<ComplaintVM>>(list);
			await HydrateNamesAsync(vms);
			return vms;
		}

		public async Task<ComplaintVM> CreateAsync(ComplaintVM vm)
		{
			if (vm.TripId.HasValue)
			{
				var existing = await _repo.GetByTripAsync(vm.TripId.Value);
				if (existing.Any(c => c.Category == vm.Category))
					throw new InvalidOperationException(
						$"A {vm.Category} report already exists for trip {vm.TripId}.");
			}

			var model = _mapper.Map<Complaint>(vm);

			if (string.IsNullOrEmpty(model.TicketReference))
				model.TicketReference = $"TKT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

			model.Status = ComplaintStatus.Open;
			model.CreatedAt = DateTime.UtcNow;

			_repo.AddAsync(model);
			await _uow.SaveChangesAsync();

			var result = _mapper.Map<ComplaintVM>(model);
			await HydrateNamesAsync(result);
			return result;
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

		public async Task<bool> AssignAgentAsync(int complaintId, string agentId)
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