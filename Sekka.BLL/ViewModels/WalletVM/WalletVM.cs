using Sekka.DAL.Models;

namespace Sekka.BLL.ViewModels.WalletVM
{
    public class WalletBalanceVM
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
    }

    public class WalletTransactionVM
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public int? RideId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
