namespace Sekka.DAL.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public Ride Ride { get; set; } = default!;
        public string PassengerId { get; set; } = default!;
        public ApplicationUser Passenger { get; set; } = default!;
        public int DriverId { get; set; }
        public Driver Driver { get; set; } = default!;
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
