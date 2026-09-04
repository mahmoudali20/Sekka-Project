using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.WalletVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> EnsureWalletAsync(string userId, CancellationToken ct = default);

        Task<WalletBalanceVM> GetBalanceAsync(string userId, CancellationToken ct = default);

        Task<Result<WalletBalanceVM>> DepositAsync(string userId, DepositVM model, CancellationToken ct = default);

        Task<IEnumerable<WalletTransactionVM>> GetTransactionsAsync(string userId, CancellationToken ct = default);

        /// <summary>
        /// Debits the payer's wallet for a ride and records the ledger row.
        /// Called by PaymentService when the chosen method is Wallet.
        /// </summary>
        Task<Result> DebitForRideAsync(string userId, int rideId, decimal amount, CancellationToken ct = default);

        /// <summary>
        /// Credits a wallet (e.g. a driver's earnings) and records the ledger row.
        /// </summary>
        Task<Result> CreditAsync(string userId, decimal amount, TransactionType type, int? rideId, string? description, CancellationToken ct = default);
    }
}
