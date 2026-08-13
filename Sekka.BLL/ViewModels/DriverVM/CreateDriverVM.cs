using Microsoft.AspNetCore.Http;
using Sekka.BLL.ViewModels.CarVM;
using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.DriverVM
{
    public class CreateDriverVM
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [DataType(DataType.EmailAddress)]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number Is Required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        [DataType(DataType.PhoneNumber)]

        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Address Is Required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public IFormFile ProfilePicture { get; set; }


        public CreateCarVM Car { get; set; } = new CreateCarVM();

    }
}
