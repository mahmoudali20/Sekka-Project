using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.PaymentVM;

namespace Sekka.BLL.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Picks the right handling for the ride's chosen PaymentMethod (Cash, Wallet, or
        /// Kashier) and initiates the payment. Wallet resolves synchronously (debits the
        /// passenger, credits the driver). Cash is recorded Pending until the driver
        /// confirms cash was collected via ConfirmCashPaymentAsync. Kashier creates a
        /// signed order and returns a RedirectUrl the caller must send the passenger to.
        /// </summary>
        /// <param name="requestBaseUrl">
        /// Scheme+host of the current request (e.g. "https://localhost:5001"), used to build
        /// Kashier's return URL. Only needed for the Kashier method.
        /// </param>
        Task<Result<PaymentVM>> InitiatePaymentAsync(InitiatePaymentVM model, string payerUserId, string? requestBaseUrl = null, CancellationToken ct = default);

        /// <summary>
        /// Driver confirms cash was collected for a ride — marks the Cash payment Completed.
        /// </summary>
        Task<Result> ConfirmCashPaymentAsync(int rideId, string driverUserId, CancellationToken ct = default);

        Task<PaymentVM?> GetByRideIdAsync(int rideId, CancellationToken ct = default);

        /// <summary>
        /// Automatically settles a completed ride when its selected payment method is Wallet.
        /// Does nothing if the ride uses another payment method or is already paid.
        /// </summary>
        Task<Result<PaymentVM>> ProcessCompletedRidePaymentAsync(int rideId, CancellationToken ct = default);

        /// <summary>
        /// Called once the passenger returns from Kashier (or the in-app demo checkout) —
        /// verifies the order and marks the matching Payment Completed or Failed. Returns
        /// the ride id on success so the caller can redirect back to that ride's payment
        /// details.
        /// </summary>
        Task<Result<int>> CompleteKashierPaymentAsync(string merchantOrderId, bool cancelled, CancellationToken ct = default);
    }
}
