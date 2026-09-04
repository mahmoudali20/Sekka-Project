
using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.PaymentVM;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL.Classes
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IKashierService _kashierService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IWalletService walletService,
            IKashierService kashierService)
        {
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _kashierService = kashierService;
        }

        public async Task<Result<PaymentVM>> InitiatePaymentAsync(
            InitiatePaymentVM model,
            string payerUserId,
            string? requestBaseUrl = null,
            CancellationToken ct = default)
        {
            var rideRepo = _unitOfWork.GetRepo<Ride, int>();

            var ride = await rideRepo.GetByIdAsync(model.RideId, ct);

            if (ride == null)
            {
                return Result<PaymentVM>.NotFound("Ride not found.");
            }

            if (ride.PassengerId != payerUserId)
            {
                return Result<PaymentVM>.Fail(
                    "You are not the passenger on this ride.",
                    ResultKind.Forbidden);
            }

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            var existingPayment = await paymentRepo.FirstOrDefaultAsync(
                p => p.RideId == ride.RideID,
                true,
                ct);

            // A failed payment can be tried again.
            // Pending or completed payments cannot be started again.
            if (existingPayment != null &&
                existingPayment.Status != PaymentStatus.Failed)
            {
                return Result<PaymentVM>.Fail(
                    "A payment has already been initiated for this ride.");
            }

            if (existingPayment != null &&
                existingPayment.Status == PaymentStatus.Failed)
            {
                paymentRepo.Delete(existingPayment);
                await _unitOfWork.SaveChangesAsync();
            }

            decimal amount;

            if (ride.ActualFare.HasValue)
            {
                amount = ride.ActualFare.Value;
            }
            else
            {
                amount = ride.EstimatedFare;
            }

            string? driverUserId = null;

            if (ride.DriverId.HasValue)
            {
                var driverRepo = _unitOfWork.GetRepo<Driver, int>();

                var driver = await driverRepo.GetByIdAsync(
                    ride.DriverId.Value,
                    ct);

                if (driver != null)
                {
                    driverUserId = driver.UserId;
                }
            }

            if (model.Method == PaymentMethod.Cash)
            {
                return await InitiateCashAsync(
                    ride.RideID,
                    amount,
                    ct);
            }

            if (model.Method == PaymentMethod.Wallet)
            {
                return await InitiateWalletAsync(
                    ride.RideID,
                    amount,
                    payerUserId,
                    driverUserId,
                    ct);
            }

            if (model.Method == PaymentMethod.Kashier)
            {
                return await InitiateKashierAsync(
                    ride.RideID,
                    amount,
                    requestBaseUrl,
                    ct);
            }

            return Result<PaymentVM>.Fail(
                "This payment method is not supported yet.");
        }

        private async Task<Result<PaymentVM>> InitiateCashAsync(
            int rideId,
            decimal amount,
            CancellationToken ct)
        {
            var payment = new Payment();

            payment.RideId = rideId;
            payment.Method = PaymentMethod.Cash;
            payment.Amount = amount;
            payment.Status = PaymentStatus.Pending;

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            paymentRepo.AddAsync(payment);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved <= 0)
            {
                return Result<PaymentVM>.Fail(
                    "Failed to record cash payment.");
            }

            var paymentVM = ToVM(
                payment,
                "Cash payment recorded — will be confirmed when the driver collects it.");

            return Result<PaymentVM>.OK(paymentVM);
        }

        private async Task<Result<PaymentVM>> InitiateWalletAsync(
            int rideId,
            decimal amount,
            string payerUserId,
            string? driverUserId,
            CancellationToken ct)
        {
            var debit = await _walletService.DebitForRideAsync(
                payerUserId,
                rideId,
                amount,
                ct);

            if (!debit.success)
            {
                return Result<PaymentVM>.Fail(
                    debit.error ?? "Failed to debit wallet.");
            }

            if (driverUserId != null)
            {
                await _walletService.CreditAsync(
                    driverUserId,
                    amount,
                    TransactionType.RideEarning,
                    rideId,
                    "Ride earning",
                    ct);
            }

            var payment = new Payment();

            payment.RideId = rideId;
            payment.Method = PaymentMethod.Wallet;
            payment.Amount = amount;
            payment.Status = PaymentStatus.Completed;

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            paymentRepo.AddAsync(payment);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved <= 0)
            {
                return Result<PaymentVM>.Fail(
                    "Failed to record wallet payment.");
            }

            var paymentVM = ToVM(payment, "Paid from wallet.");

            return Result<PaymentVM>.OK(paymentVM);
        }

        private async Task<Result<PaymentVM>> InitiateKashierAsync(
            int rideId,
            decimal amount,
            string? requestBaseUrl,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(requestBaseUrl))
            {
                return Result<PaymentVM>.Fail(
                    "Could not determine the site URL for the Kashier redirect.");
            }

            var order = _kashierService.CreateOrder(
                rideId,
                amount,
                requestBaseUrl);

            if (!order.Success)
            {
                return Result<PaymentVM>.Fail(
                    order.Error ?? "Failed to start Kashier payment.");
            }

            var payment = new Payment();

            payment.RideId = rideId;
            payment.Method = PaymentMethod.Kashier;
            payment.Amount = amount;
            payment.Status = PaymentStatus.Pending;

            // Save the Kashier order ID so we can find this payment later.
            payment.ProviderReference = order.OrderId;

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            paymentRepo.AddAsync(payment);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved <= 0)
            {
                return Result<PaymentVM>.Fail(
                    "Failed to record Kashier payment.");
            }

            var paymentVM = ToVM(
                payment,
                "Redirecting to Kashier…");

            paymentVM.RedirectUrl = order.ApproveUrl;

            return Result<PaymentVM>.OK(paymentVM);
        }

        public async Task<Result<int>> CompleteKashierPaymentAsync(
            string merchantOrderId,
            bool cancelled,
            CancellationToken ct = default)
        {
            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            var payment = await paymentRepo.FirstOrDefaultAsync(
                p => p.ProviderReference == merchantOrderId &&
                     p.Method == PaymentMethod.Kashier,
                true,
                ct);

            if (payment == null)
            {
                return Result<int>.NotFound(
                    "No matching Kashier payment found.");
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                return Result<int>.OK(payment.RideId);
            }

            if (cancelled)
            {
                payment.Status = PaymentStatus.Failed;
            }
            else
            {
                payment.Status = PaymentStatus.Completed;
            }

            paymentRepo.Update(payment);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved > 0)
            {
                return Result<int>.OK(payment.RideId);
            }

            return Result<int>.Fail(
                "Failed to update Kashier payment.");
        }

        public async Task<Result> ConfirmCashPaymentAsync(
            int rideId,
            string driverUserId,
            CancellationToken ct = default)
        {
            var rideRepo = _unitOfWork.GetRepo<Ride, int>();

            var ride = await rideRepo.GetByIdAsync(rideId, ct);

            if (ride == null)
            {
                return Result.NotFound("Ride not found.");
            }

            if (!ride.DriverId.HasValue)
            {
                return Result.Fail(
                    "This ride has no assigned driver.");
            }

            var driverRepo = _unitOfWork.GetRepo<Driver, int>();

            var driver = await driverRepo.GetByIdAsync(
                ride.DriverId.Value,
                ct);

            if (driver == null || driver.UserId != driverUserId)
            {
                return Result.Fail(
                    "You are not the driver on this ride.",
                    ResultKind.Forbidden);
            }

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            var payment = await paymentRepo.FirstOrDefaultAsync(
                p => p.RideId == rideId,
                true,
                ct);

            if (payment == null)
            {
                return Result.NotFound(
                    "No payment found for this ride.");
            }

            if (payment.Method != PaymentMethod.Cash)
            {
                return Result.Fail(
                    "Only Cash payments need manual confirmation.");
            }

            payment.Status = PaymentStatus.Completed;

            paymentRepo.Update(payment);

            var saved = await _unitOfWork.SaveChangesAsync();

            if (saved > 0)
            {
                return Result.OK();
            }

            return Result.Fail(
                "Failed to confirm cash payment.");
        }

        public async Task<Result<PaymentVM>> ProcessCompletedRidePaymentAsync(
            int rideId,
            CancellationToken ct = default)
        {
            var rideRepo = _unitOfWork.GetRepo<Ride, int>();

            var ride = await rideRepo.GetByIdAsync(rideId, ct);

            if (ride == null)
            {
                return Result<PaymentVM>.NotFound(
                    "Ride not found.");
            }

            if (ride.Status != RideStatus.Completed)
            {
                return Result<PaymentVM>.Fail(
                    "The ride must be completed before payment can be processed.");
            }

            // Only Wallet payments are automatically processed here.
            // Cash needs driver confirmation.
            // Kashier uses its own checkout and callback.
            if (ride.PreferredPaymentMethod != PaymentMethod.Wallet)
            {
                var paymentVM = new PaymentVM();

                paymentVM.RideId = rideId;
                paymentVM.Method =
                    ride.PreferredPaymentMethod ?? PaymentMethod.Cash;

                if (ride.ActualFare.HasValue)
                {
                    paymentVM.Amount = ride.ActualFare.Value;
                }
                else
                {
                    paymentVM.Amount = ride.EstimatedFare;
                }

                paymentVM.Status = PaymentStatus.Pending;

                paymentVM.Message =
                    "No automatic wallet payment is required for this ride.";

                return Result<PaymentVM>.OK(paymentVM);
            }

            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            var existingPayment = await paymentRepo.FirstOrDefaultAsync(
                p => p.RideId == rideId,
                true,
                ct);

            // If the wallet was already charged, do not charge it again.
            if (existingPayment != null &&
                existingPayment.Method == PaymentMethod.Wallet &&
                existingPayment.Status == PaymentStatus.Completed)
            {
                return Result<PaymentVM>.OK(
                    ToVM(
                        existingPayment,
                        "Wallet payment was already completed."));
            }

            if (existingPayment != null &&
                existingPayment.Status != PaymentStatus.Failed)
            {
                return Result<PaymentVM>.Fail(
                    "A payment has already been initiated for this ride.");
            }

            if (existingPayment != null &&
                existingPayment.Status == PaymentStatus.Failed)
            {
                paymentRepo.Delete(existingPayment);
                await _unitOfWork.SaveChangesAsync();
            }

            decimal amount;

            if (ride.ActualFare.HasValue)
            {
                amount = ride.ActualFare.Value;
            }
            else
            {
                amount = ride.EstimatedFare;
            }

            if (amount <= 0)
            {
                return Result<PaymentVM>.Validation(
                    "Ride payment amount must be greater than zero.");
            }

            string? driverUserId = null;

            if (ride.DriverId.HasValue)
            {
                var driverRepo = _unitOfWork.GetRepo<Driver, int>();

                var driver = await driverRepo.GetByIdAsync(
                    ride.DriverId.Value,
                    ct);

                if (driver != null)
                {
                    driverUserId = driver.UserId;
                }
            }

            return await InitiateWalletAsync(
                rideId,
                amount,
                ride.PassengerId,
                driverUserId,
                ct);
        }

        public async Task<PaymentVM?> GetByRideIdAsync(
            int rideId,
            CancellationToken ct = default)
        {
            var paymentRepo = _unitOfWork.GetRepo<Payment, int>();

            var payment = await paymentRepo.FirstOrDefaultAsync(
                p => p.RideId == rideId,
                false,
                ct);

            if (payment == null)
            {
                return null;
            }

            return ToVM(payment, null);
        }

        private static PaymentVM ToVM(
            Payment payment,
            string? message)
        {
            var result = new PaymentVM();

            result.Id = payment.Id;
            result.RideId = payment.RideId;
            result.Method = payment.Method;
            result.Amount = payment.Amount;
            result.Status = payment.Status;
            result.Message = message;

            return result;
        }
    }
}

