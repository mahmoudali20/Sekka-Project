using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
    public interface ITripNotificationService
    {
        Task NotifyNewRideAsync(Ride ride);

        Task NotifyRideAcceptedAsync(Ride ride, Driver driver);

        Task NotifyRideStartedAsync(Ride ride);

        Task NotifyRideCompletedAsync(Ride ride);

        Task NotifyRideCancelledAsync(Ride ride, string? driverUserId = null);
    }
}