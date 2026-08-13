namespace Sekka.DAL.Models
{
    public class Driver
    {

        public int Id { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal RatingAverage { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public int CarId { get; set; }
        public Car Car { get; set; } = default!;




    }
}
