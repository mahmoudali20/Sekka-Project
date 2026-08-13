namespace Sekka.BLL.ViewModels.DriverVM
{
    public class DriverDetailsVM
    {
        public int Id { get; set; }

        // Driver
        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public decimal RatingAverage { get; set; }

        // User
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }

        // Car
        public int CarId { get; set; }
        public string CarModel { get; set; } = string.Empty;
        public string CarColor { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string? CarImage { get; set; }
    }
}
