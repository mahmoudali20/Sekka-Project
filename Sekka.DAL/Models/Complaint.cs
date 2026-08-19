using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sekka.DAL.Models
{
	public enum ComplaintCategory
	{
		FareDispute = 1,
		DriverBehavior,
		RiderBehavior,
		VehicleCondition,
		SafetyIncident,
		LostItem,
		AppTechnicalIssue
	}

	public enum ComplaintStatus
	{
		Open = 1,
		InReview,
		Escalated,
		Resolved,
		Closed
	}

	public enum ComplaintPriority
	{
		Low = 1,
		Medium,
		High,
		Critical
	}

	public class Complaint
	{
		public int Id { get; set; }
		public string TicketReference { get; set; } = string.Empty;

		public int ComplainantId { get; set; }
		public int? TripId { get; set; }
		public int? TargetUserId { get; set; }
		public int? AssignedAgentId { get; set; }

		public ComplaintCategory Category { get; set; }
		public ComplaintPriority Priority { get; set; }
		public ComplaintStatus Status { get; set; }

		public string Subject { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string? ResolutionNotes { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? ResolvedAt { get; set; }
	}
}