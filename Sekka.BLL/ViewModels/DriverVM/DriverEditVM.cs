namespace Sekka.BLL.ViewModels.DriverVM
{
    public class DriverEditVM
    {



        // Driver Data
        public string LicenseNumber { get; set; } = string.Empty;

        // ApplicationUser Data
        public string Address { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
