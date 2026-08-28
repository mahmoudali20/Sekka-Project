namespace Sekka.DAL.Models
{
    public class Ride
    {
        public int RideID { get; set; }
        public string PassengerId { get; set; }
        public ApplicationUser Passenger { get; set; }
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public string PickupLocation { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public string DropoffLocation { get; set; }
        public double DropoffLat { get; set; }
        public double DropoffLng { get; set; }

        public double DistanceInKm { get; set; }
        public decimal EstimatedFare { get; set; }
        public decimal? ActualFare { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Requested;
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public bool IsCancelled { get; set; }
        public string? CancelReason { get; set; }
        public Rating? Rating { get; set; }
    }
}
