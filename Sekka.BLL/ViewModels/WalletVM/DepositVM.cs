using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.WalletVM
{
    public class DepositVM
    {
        [Range(1, double.MaxValue, ErrorMessage = "Deposit amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
