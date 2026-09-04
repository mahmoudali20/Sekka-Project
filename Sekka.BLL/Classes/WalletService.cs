
using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.WalletVM;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL.Classes
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Wallet> EnsureWalletAsync(
            string userId,
            CancellationToken ct = default)
        {
            var walletRepo = _unitOfWork.GetRepo<Wallet, int>();

            var wallet = await walletRepo.FirstOrDefaultAsync(
                w => w.UserId == userId,
                true,
                ct);

            if (wallet != null)
            {
                return wallet;
            }

            wallet = new Wallet();
            wallet.UserId = userId;
            wallet.Balance = 0m;

            walletRepo.AddAsync(wallet);
            await _unitOfWork.SaveChangesAsync();

            return wallet;
        }

        public async Task<WalletBalanceVM> GetBalanceAsync(
            string userId,
            CancellationToken ct = default)
        {
            var wallet = await EnsureWalletAsync(userId, ct);

            var balance = new WalletBalanceVM();
            balance.WalletId = wallet.Id;
            balance.Balance = wallet.Balance;

            return balance;
        }

        public async Task<Result<WalletBalanceVM>> DepositAsync(
            string userId,
            DepositVM model,
            CancellationToken ct = default)
        {
            if (model.Amount <= 0)
            {
                return Result<WalletBalanceVM>.Validation(
                    "Deposit amount must be greater than zero.");
            }

            var wallet = await EnsureWalletAsync(userId, ct);

            wallet.Balance = wallet.Balance + model.Amount;

            var walletRepo = _unitOfWork.GetRepo<Wallet, int>();
            walletRepo.Update(wallet);

            var transaction = new WalletTransaction();
            transaction.WalletId = wallet.Id;
            transaction.Type = TransactionType.Deposit;
            transaction.Amount = model.Amount;
            transaction.BalanceAfter = wallet.Balance;
            transaction.Description = model.Description ?? "Wallet deposit";

            var transactionRepo =
                _unitOfWork.GetRepo<WalletTransaction, int>();

            transactionRepo.AddAsync(transaction);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved <= 0)
            {
                return Result<WalletBalanceVM>.Fail(
                    "Failed to deposit into wallet.");
            }

            var result = new WalletBalanceVM();
            result.WalletId = wallet.Id;
            result.Balance = wallet.Balance;

            return Result<WalletBalanceVM>.OK(result);
        }

        public async Task<IEnumerable<WalletTransactionVM>> GetTransactionsAsync(
            string userId,
            CancellationToken ct = default)
        {
            var wallet = await EnsureWalletAsync(userId, ct);

            var transactionRepo =
                _unitOfWork.GetRepo<WalletTransaction, int>();

            var transactions =
                await transactionRepo.GetAllAsync(false, ct);

            var userTransactions = transactions
                .Where(t => t.WalletId == wallet.Id)
                .OrderByDescending(t => t.CreatedAt);

            var result = new List<WalletTransactionVM>();

            foreach (var transaction in userTransactions)
            {
                var item = new WalletTransactionVM();

                item.Id = transaction.Id;
                item.Type = transaction.Type;
                item.Amount = transaction.Amount;
                item.BalanceAfter = transaction.BalanceAfter;
                item.Description = transaction.Description;
                item.RideId = transaction.RideId;
                item.CreatedAt = transaction.CreatedAt;

                result.Add(item);
            }

            return result;
        }

        public async Task<Result> DebitForRideAsync(
            string userId,
            int rideId,
            decimal amount,
            CancellationToken ct = default)
        {
            if (amount <= 0)
            {
                return Result.Validation(
                    "Ride payment amount must be greater than zero.");
            }

            var wallet = await EnsureWalletAsync(userId, ct);

            if (wallet.Balance < amount)
            {
                return Result.Fail(
                    "Insufficient wallet balance for this ride.");
            }

            wallet.Balance = wallet.Balance - amount;

            var walletRepo = _unitOfWork.GetRepo<Wallet, int>();
            walletRepo.Update(wallet);

            var transaction = new WalletTransaction();

            transaction.WalletId = wallet.Id;
            transaction.Type = TransactionType.RidePayment;
            transaction.Amount = amount;
            transaction.BalanceAfter = wallet.Balance;
            transaction.Description = "Ride payment";
            transaction.RideId = rideId;

            var transactionRepo =
                _unitOfWork.GetRepo<WalletTransaction, int>();

            transactionRepo.AddAsync(transaction);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved > 0)
            {
                return Result.OK();
            }

            return Result.Fail("Failed to debit wallet.");
        }

        public async Task<Result> CreditAsync(
            string userId,
            decimal amount,
            TransactionType type,
            int? rideId,
            string? description,
            CancellationToken ct = default)
        {
            if (amount <= 0)
            {
                return Result.Validation(
                    "Credit amount must be greater than zero.");
            }

            var wallet = await EnsureWalletAsync(userId, ct);

            wallet.Balance = wallet.Balance + amount;

            var walletRepo = _unitOfWork.GetRepo<Wallet, int>();
            walletRepo.Update(wallet);

            var transaction = new WalletTransaction();

            transaction.WalletId = wallet.Id;
            transaction.Type = type;
            transaction.Amount = amount;
            transaction.BalanceAfter = wallet.Balance;
            transaction.Description = description;
            transaction.RideId = rideId;

            var transactionRepo =
                _unitOfWork.GetRepo<WalletTransaction, int>();

            transactionRepo.AddAsync(transaction);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved > 0)
            {
                return Result.OK();
            }

            return Result.Fail("Failed to credit wallet.");
        }
    }
}




