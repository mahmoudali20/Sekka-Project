using Microsoft.AspNetCore.SignalR;
using Sekka.BLL.Interfaces;
using Sekka.DAL.Models;
using Sekka.PL.Hubs;

namespace Sekka.PL.Services
{
    public class TripNotificationService : ITripNotificationService
    {
        private readonly IHubContext<TripHub> _hubContext;

        public TripNotificationService(IHubContext<TripHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewRideAsync(Ride ride)
        {
            await _hubContext.Clients.Group("Drivers").SendAsync("NewRide", new
            {
                rideId = ride.RideID,
                pickupLocation = ride.PickupLocation,
                dropoffLocation = ride.DropoffLocation,

                distanceInKm = ride.DistanceInKm,
                estimatedFare = ride.EstimatedFare,

                passengerName = ride.Passenger?.FullName ?? "New Passenger",

                requestTime = ride.RequestTime.ToLocalTime().ToString("hh:mm tt")
            });
        }

        public async Task NotifyRideAcceptedAsync(Ride ride, Driver driver)
        {

            await _hubContext.Clients.User(ride.PassengerId).SendAsync("RideAccepted", new
            {
                rideId = ride.RideID,
                driverName = driver.User?.FullName ?? "Unknown",
                driverPhone = driver.User?.PhoneNumber ?? "N/A",
                driverRate = driver.RatingAverage,
                carColor = driver.Car?.Color ?? "",
                carModel = driver.Car?.Model ?? "Unknown Car",
                carPlate = driver.Car?.PlateNumber ?? "N/A",
                status = RideStatus.Accepted.ToString(),
                message = "Your driver has accepted your ride."
            });


            await _hubContext.Clients.Group("Drivers").SendAsync("RemoveAvailableRide", new
            {
                rideId = ride.RideID
            });
        }

        public async Task NotifyRideStartedAsync(Ride ride)
        {
            await _hubContext.Clients.User(ride.PassengerId).SendAsync("RideStarted", new
            {
                rideId = ride.RideID,
                status = ride.Status.ToString(),
                startTime = ride.StartTime?.ToLocalTime().ToString("hh:mm tt") ?? ""
            });
        }

        public async Task NotifyRideCompletedAsync(Ride ride)
        {

            string rideDuration = "Unknown";
            if (ride.StartTime.HasValue && ride.EndTime.HasValue)
            {
                TimeSpan duration = ride.EndTime.Value - ride.StartTime.Value;

                if (duration.TotalHours >= 1)
                    rideDuration = $"{(int)duration.TotalHours}h {duration.Minutes}m";
                else
                    rideDuration = $"{duration.Minutes} mins";
            }

            await _hubContext.Clients.User(ride.PassengerId).SendAsync("RideCompleted", new
            {
                rideId = ride.RideID,
                status = ride.Status.ToString(),
                actualFare = ride.ActualFare,
                endTime = ride.EndTime?.ToLocalTime().ToString("hh:mm tt") ?? "",
                duration = rideDuration
            });
        }

        public async Task NotifyRideCancelledAsync(Ride ride, string? driverUserId)
        {

            if (string.IsNullOrEmpty(driverUserId))
            {

                await _hubContext.Clients.Group("Drivers").SendAsync("RemoveAvailableRide", new
                {
                    rideId = ride.RideID
                });
            }

            else
            {
                await _hubContext.Clients.User(driverUserId).SendAsync("RideCancelled", new
                {
                    rideId = ride.RideID,
                    message = "The passenger has cancelled the ride."
                });
            }
        }

    }
}