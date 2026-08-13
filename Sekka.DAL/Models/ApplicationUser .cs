using Microsoft.AspNetCore.Identity;

namespace Sekka.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = default!;
        public string? ProfilePicture { get; set; }

        public Driver? Driver { get; set; }
    }
}
