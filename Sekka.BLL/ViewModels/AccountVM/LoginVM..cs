using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.AccountVM
{
    public class LoginVM
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
