using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels;
using Sekka.BLL.ViewModels.TripVM;
using Sekka.DAL.Models;

namespace Sekka.PL.Controllers
{
	[Authorize(Roles = "Passenger")]
	public class RideHistoryController : Controller
	{
		private readonly ITripService _rideService;
		private readonly IComplaintService _complaintService;
		private readonly UserManager<ApplicationUser> _userManager;

		public RideHistoryController(
			ITripService rideService,
			IComplaintService complaintService,
			UserManager<ApplicationUser> userManager)
		{
			_rideService = rideService;
			_complaintService = complaintService;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var userId = _userManager.GetUserId(User)!;
			var rides = await _rideService.GetByPassengerAsync(userId);

			var allComplaints = await _complaintService.GetByComplainantAsync(userId);

			var complaintsByRide = allComplaints
				.Where(c => c.TripId.HasValue)
				.GroupBy(c => c.TripId!.Value)
				.ToDictionary(
					g => g.Key,
					g => new HashSet<ComplaintCategory>(g.Select(c => c.Category))
				);

			var complaintDetail = allComplaints
				.Where(c => c.TripId.HasValue)
				.ToDictionary(
					c => (c.TripId!.Value, c.Category),
					c => c
				);

			ViewBag.ComplaintsByRide = complaintsByRide;
			ViewBag.ComplaintDetail = complaintDetail;

			return View(rides);
		}

		[HttpGet]
		public async Task<IActionResult> FileComplaint(int rideId, int category = (int)ComplaintCategory.DriverBehavior)
		{
			var ride = await _rideService.GetRideByIdAsync(rideId);
			if (ride is null) return NotFound();

			if (ride.Status != RideStatus.Completed && ride.Status != RideStatus.Cancelled)
				return BadRequest("You can only file a complaint on completed or cancelled rides.");

			var parsedCategory = (ComplaintCategory)category;

			var existing = await _complaintService.GetByTripAsync(rideId);
			if (existing.Any(c => c.Category == parsedCategory))
			{
				TempData["Info"] = $"You have already submitted a {parsedCategory.ToString().Replace("_", " ")} report for this ride.";
				return RedirectToAction(nameof(Index));
			}

			var vm = new ComplaintVM
			{
				TripId = rideId,
				Category = parsedCategory,
				Priority = ComplaintPriority.Medium,
				Status = ComplaintStatus.Open,
				ComplainantId = _userManager.GetUserId(User)!
			};

			ViewBag.RideId = rideId;
			ViewBag.Category = parsedCategory;
			return View("ComplaintForm", vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> FileComplaint(ComplaintVM vm)
		{
			var currentUserId = _userManager.GetUserId(User)!;
			vm.ComplainantId = currentUserId;
			vm.Status = ComplaintStatus.Open;

			ModelState.Remove(nameof(vm.ComplainantId));
			ModelState.Remove(nameof(vm.Status));
			ModelState.Remove(nameof(vm.Priority));

			if (!ModelState.IsValid)
			{
				ViewBag.RideId = vm.TripId;
				ViewBag.Category = vm.Category;
				return View("ComplaintForm", vm);
			}

			if (vm.TripId.HasValue)
			{
				var existing = await _complaintService.GetByTripAsync(vm.TripId.Value);
				if (existing.Any(c => c.Category == vm.Category))
				{
					TempData["Info"] = $"You have already submitted a {vm.Category.ToString().Replace("_", " ")} report for this ride.";
					return RedirectToAction(nameof(Index));
				}
			}

			try
			{
				var created = await _complaintService.CreateAsync(vm);

				TempData["Success"] = vm.Category == ComplaintCategory.LostItem
					? $"Lost-item report submitted. Ticket: {created.TicketReference}"
					: $"Complaint submitted. Ticket: {created.TicketReference}";
			}
			catch (InvalidOperationException)
			{
				TempData["Info"] = $"You have already submitted a {vm.Category.ToString().Replace("_", " ")} report for this ride.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> EditComplaint(int id)
		{
			var vm = await _complaintService.GetByIdAsync(id);
			if (vm is null)
			{
				TempData["Error"] = "Complaint not found.";
				return RedirectToAction(nameof(Index));
			}

			var currentUserId = _userManager.GetUserId(User)!;
			if (vm.ComplainantId != currentUserId)
			{
				TempData["Error"] = "You don't have permission to edit this complaint.";
				return RedirectToAction(nameof(Index));
			}

			if (vm.Status != ComplaintStatus.Open && vm.Status != ComplaintStatus.InReview)
			{
				TempData["Info"] = "Only open or in-review complaints can be edited.";
				return RedirectToAction(nameof(Index));
			}


			ViewBag.RideId = vm.TripId;
			ViewBag.Category = vm.Category;
			return View("ComplaintForm", vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditComplaint(ComplaintVM vm)
		{
			var currentUserId = _userManager.GetUserId(User)!;

			var existing = await _complaintService.GetByIdAsync(vm.Id);
			if (existing is null) return NotFound();
			if (existing.ComplainantId != currentUserId) return Forbid();
			if (existing.Status != ComplaintStatus.Open && existing.Status != ComplaintStatus.InReview)
			{
				TempData["Info"] = "Only open or in-review complaints can be edited.";
				return RedirectToAction(nameof(Index));
			}

			vm.ComplainantId = currentUserId;
			vm.Status = existing.Status;
			vm.Category = existing.Category;
			vm.TripId = existing.TripId;
			vm.TicketReference = existing.TicketReference;
			vm.CreatedAt = existing.CreatedAt;

			ModelState.Remove(nameof(vm.ComplainantId));
			ModelState.Remove(nameof(vm.Status));
			ModelState.Remove(nameof(vm.Priority));

			if (!ModelState.IsValid)
			{
				ViewBag.RideId = vm.TripId;
				ViewBag.Category = vm.Category;
				return View("ComplaintForm", vm);
			}

			var updated = await _complaintService.UpdateAsync(vm);
			TempData[updated ? "Success" : "Error"] = updated
				? "Your complaint has been updated."
				: "Update failed — please try again.";

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteComplaint(int id)
		{
			var existing = await _complaintService.GetByIdAsync(id);
			if (existing is null) return NotFound();
			if (existing.ComplainantId != _userManager.GetUserId(User)) return Forbid();

			if (existing.Status != ComplaintStatus.Open && existing.Status != ComplaintStatus.InReview)
			{
				TempData["Info"] = "Only open or in-review complaints can be deleted.";
				return RedirectToAction(nameof(Index));
			}

			var deleted = await _complaintService.DeleteAsync(id);
			TempData[deleted ? "Success" : "Error"] = deleted
				? "Complaint withdrawn successfully."
				: "Delete failed — please try again.";

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RateDriver(RateDriverVM model)
		{
			if (model.Score < 1 || model.Score > 5)
			{
				TempData["Error"] = "Please select a rating between 1 and 5 stars.";
				return RedirectToAction(nameof(Index));
			}

			var passengerId = _userManager.GetUserId(User)!;
			var result = await _rideService.RateDriverAsync(model, passengerId);

			TempData[result.success ? "Success" : "Error"] = result.success
				? "Thank you! Your rating has been submitted."
				: result.error;

			return RedirectToAction(nameof(Index));
		}
	}
}