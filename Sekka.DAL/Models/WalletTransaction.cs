namespace Sekka.DAL.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }

        public int WalletId { get; set; }
        public Wallet Wallet { get; set; } = default!;

        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }

        
        public decimal BalanceAfter { get; set; }

        public string? Description { get; set; }

      
        public int? RideId { get; set; }
        public Ride? Ride { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
