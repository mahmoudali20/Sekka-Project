using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels;
using Sekka.DAL.Models;
using System.Security.Claims;

namespace Sekka.PL.Controllers
{
	[Authorize(Roles = "SuperAdmin,Admin")]
	public class ComplaintsController : Controller
	{
		private readonly IComplaintService _complaintService;
		private readonly IAccountService _accountService;
		private readonly IComplaintAiService _aiService;

		public ComplaintsController(IComplaintService complaintService, IAccountService accountService, IComplaintAiService aiService)
		{
			_complaintService = complaintService;
			_accountService = accountService;
			_aiService = aiService;
		}

		[Authorize(Roles = "SuperAdmin")]
		public async Task<IActionResult> Index()
		{
			var complaints = await _complaintService.GetAllAsync();
			return View(complaints);
		}

		[Authorize(Roles = "SuperAdmin")]
		[HttpGet]
		public async Task<IActionResult> Assign(int id)
		{
			var complaint = await _complaintService.GetByIdAsync(id);
			if (complaint is null) return NotFound();

			var adminsResult = await _accountService.GetUsersInRoleAsync("Admin");
			var admins = adminsResult.success ? adminsResult.Value! : Enumerable.Empty<ApplicationUser>();

			ViewBag.AdminList = admins.Select(a => new SelectListItem
			{
				Value = a.Id,
				Text = $"{a.FullName} ({a.UserName})"
			}).ToList();

			return View(complaint);
		}

		[Authorize(Roles = "SuperAdmin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Assign(int complaintId, string adminId)
		{
			if (string.IsNullOrWhiteSpace(adminId))
			{
				TempData["ErrorMessage"] = "Please select an admin.";
				return RedirectToAction(nameof(Assign), new { id = complaintId });
			}

			var userResult = await _accountService.FindByIdAsync(adminId);
			if (!userResult.success)
			{
				TempData["ErrorMessage"] = "Selected admin not found.";
				return RedirectToAction(nameof(Assign), new { id = complaintId });
			}

			var isAdmin = await _accountService.IsInRoleAsync(userResult.Value!, "Admin");
			if (!isAdmin.success || !isAdmin.Value)
			{
				TempData["ErrorMessage"] = "Selected user is not an Admin.";
				return RedirectToAction(nameof(Assign), new { id = complaintId });
			}

			var ok = await _complaintService.AssignAgentAsync(complaintId, adminId);
			if (!ok)
			{
				TempData["ErrorMessage"] = "Failed to assign the ticket.";
				return RedirectToAction(nameof(Assign), new { id = complaintId });
			}

			TempData["SuccessMessage"] = $"Ticket assigned to {userResult.Value!.FullName}.";
			return RedirectToAction(nameof(Index));
		}
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> MyTickets()
		{
			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			var tickets = await _complaintService.GetByAgentAsync(currentUserId);
			return View(tickets);
		}
		[HttpGet]
		public async Task<IActionResult> Details(int id)
		{
			var complaint = await _complaintService.GetByIdAsync(id);
			if (complaint is null) return NotFound();

			if (User.IsInRole("Admin"))
			{
				var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
				if (complaint.AssignedAgentId != currentUserId)
					return Forbid();
			}

			ViewBag.StatusOptions = Enum.GetValues<ComplaintStatus>()
				.Select(s => new SelectListItem
				{
					Value = ((int)s).ToString(),
					Text = s.ToString()
				}).ToList();

			if (_aiService.IsConfigured)
			{
				try
				{
					ViewBag.AiSummary = await _aiService.SummarizeAsync(complaint.Description);
					ViewBag.AiCategory = await _aiService.ClassifyAsync(complaint.Description);
				}
				catch (Exception ex)
				{
					ViewBag.AiError = ex.Message;
				}
			}

			return View(complaint);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(int complaintId, ComplaintStatus newStatus)
		{
			var complaint = await _complaintService.GetByIdAsync(complaintId);
			if (complaint is null) return NotFound();

			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			if (complaint.AssignedAgentId != currentUserId)
				return Forbid();

			var ok = await _complaintService.UpdateStatusAsync(complaintId, newStatus);

			TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
				ok ? "Status updated successfully." : "Failed to update status.";

			return RedirectToAction(nameof(MyTickets));
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Resolve(int complaintId, string resolutionNotes)
		{
			if (string.IsNullOrWhiteSpace(resolutionNotes))
			{
				TempData["ErrorMessage"] = "Resolution notes are required.";
				return RedirectToAction(nameof(Details), new { id = complaintId });
			}

			var complaint = await _complaintService.GetByIdAsync(complaintId);
			if (complaint is null) return NotFound();

			var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			if (complaint.AssignedAgentId != currentUserId)
				return Forbid();

			var ok = await _complaintService.ResolveAsync(complaintId, resolutionNotes);

			TempData[ok ? "SuccessMessage" : "ErrorMessage"] =
				ok ? "Ticket resolved successfully." : "Failed to resolve ticket.";

			return RedirectToAction(nameof(MyTickets));
		}
	}
}