using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.CarVM
{
    public class CreateCarVM
    {
        [Required]
        public string Model { get; set; } = string.Empty;
        [Required]
        public string Color { get; set; } = string.Empty;
        [Required]
        public string PlateNumber { get; set; } = string.Empty;
        [Required]
        public string? LicensePlate { get; set; }
        [Required]
        public IFormFile ProfilePicture { get; set; }



    }
}
