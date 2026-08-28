using Sekka.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels
{
	public class ComplaintVM
	{
		public int Id { get; set; }

		public string TicketReference { get; set; } = string.Empty;

		[Required]
		public string ComplainantId { get; set; } = string.Empty;

		public int? TripId { get; set; }

		public string? TargetUserId { get; set; }

		public string? AssignedAgentId { get; set; }

		[Required]
		public ComplaintCategory Category { get; set; }

		[Required]
		public ComplaintPriority Priority { get; set; }

		public ComplaintStatus Status { get; set; }

		[Required, MaxLength(200)]
		public string Subject { get; set; } = string.Empty;

		[Required, MaxLength(2000)]
		public string Description { get; set; } = string.Empty;

		public string? ResolutionNotes { get; set; }

		public DateTime CreatedAt { get; set; }
		public DateTime? ResolvedAt { get; set; }

		public string CategoryDisplay { get; set; } = string.Empty;
		public string PriorityDisplay { get; set; } = string.Empty;
		public string StatusDisplay { get; set; } = string.Empty;
	}
}