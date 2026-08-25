using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.TripVM;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL.Classes
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Ride>> BookRideAsync(BookRideVM model, CancellationToken ct = default)
        {
            bool hasActiveRide = await _unitOfWork.GetRepo<Ride, int>().AnyAsync(r =>
                r.PassengerId == model.PassengerId &&
                r.Status != RideStatus.Completed &&
                r.Status != RideStatus.Cancelled, ct);

            if (hasActiveRide) return Result<Ride>.Fail("You already have an active ride.");

            var ride = new Ride
            {
                PassengerId = model.PassengerId,
                PickupLocation = model.PickupLocation,
                PickupLat = model.PickupLat,
                PickupLng = model.PickupLng,
                DropoffLocation = model.DropoffLocation,
                DropoffLat = model.DropoffLat,
                DropoffLng = model.DropoffLng,
                DistanceInKm = model.DistanceInKm,
                EstimatedFare = model.EstimatedFare,
                ScheduledTime = model.ScheduledTime,
                Status = RideStatus.Requested,
                RequestTime = DateTime.UtcNow,
                IsCancelled = false
            };

            _unitOfWork.GetRepo<Ride, int>().AddAsync(ride);
            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result<Ride>.OK(ride) : Result<Ride>.Fail("Failed to book the ride.");
        }

        public async Task<Result> CancelRideAsync(int rideId, string passengerId, string reason, CancellationToken ct = default)
        {
            var ride = await _unitOfWork.GetRepo<Ride, int>().GetByIdAsync(rideId, ct);

            if (ride == null)
                return Result.Fail("Ride not found.");

            if (ride.PassengerId != passengerId)
                return Result.Fail("Unauthorized to cancel this ride.", ResultKind.Forbidden);

            if (ride.Status == RideStatus.Started || ride.Status == RideStatus.Completed)
                return Result.Fail("Cannot cancel a ride that is already in progress or completed.");

            ride.IsCancelled = true;
            ride.CancelReason = reason;
            ride.Status = RideStatus.Cancelled;

            _unitOfWork.GetRepo<Ride, int>().Update(ride);
            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result.OK() : Result.Fail("Failed to cancel ride.");
        }

        public async Task<Result> AcceptRideAsync(int rideId, string driverUserId, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().FirstOrDefaultAsync(d => d.UserId == driverUserId, false, ct);
            if (driver == null) return Result.Fail("Driver profile not found.");

            var ride = await _unitOfWork.GetRepo<Ride, int>().GetByIdAsync(rideId, ct);
            if (ride == null) return Result.Fail("Ride not found.");

            if (ride.Status != RideStatus.Requested) return Result.Fail("Ride is no longer available.", ResultKind.Conflict);

            ride.DriverId = driver.Id;
            ride.Status = RideStatus.Accepted;

            _unitOfWork.GetRepo<Ride, int>().Update(ride);
            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result.OK() : Result.Fail("Failed to accept ride.");
        }

        public async Task<Result> StartRideAsync(int rideId, string driverUserId, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().FirstOrDefaultAsync(d => d.UserId == driverUserId, false, ct);
            if (driver == null) return Result.Fail("Driver profile not found.");

            var ride = await _unitOfWork.GetRepo<Ride, int>().GetByIdAsync(rideId, ct);
            if (ride == null) return Result.Fail("Ride not found.");

            if (ride.DriverId != driver.Id) return Result.Fail("You are not authorized to start this ride.", ResultKind.Forbidden);

            if (ride.Status != RideStatus.Accepted && ride.Status != RideStatus.DriverArrived)
                return Result.Fail("Invalid ride status to start.");

            ride.Status = RideStatus.Started;
            ride.StartTime = DateTime.UtcNow;

            _unitOfWork.GetRepo<Ride, int>().Update(ride);
            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result.OK() : Result.Fail("Failed to start ride.");
        }

        public async Task<Result> CompleteRideAsync(int rideId, string driverUserId, decimal actualFare, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().FirstOrDefaultAsync(d => d.UserId == driverUserId, false, ct);
            if (driver == null) return Result.Fail("Driver profile not found.");

            var ride = await _unitOfWork.GetRepo<Ride, int>().GetByIdAsync(rideId, ct);
            if (ride == null) return Result.Fail("Ride not found.");
            if (ride.DriverId != driver.Id) return Result.Fail("You are not authorized to complete this ride.", ResultKind.Forbidden);
            if (ride.Status != RideStatus.Started) return Result.Fail("Ride must be started before it can be completed.");

            ride.Status = RideStatus.Completed;
            ride.EndTime = DateTime.UtcNow;
            ride.ActualFare = actualFare;
            driver.IsAvailable = true;

            _unitOfWork.GetRepo<Ride, int>().Update(ride);
            _unitOfWork.GetRepo<Driver, int>().Update(driver);

            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result.OK() : Result.Fail("Failed to complete ride.");
        }

        public async Task<IEnumerable<Ride>> GetAllRidesAsync(CancellationToken ct = default)
        {
            var rides = (await _unitOfWork.GetRepo<Ride, int>().GetAllAsync(false, ct))?.ToList() ?? new List<Ride>();
            var drivers = (await _unitOfWork.DriverRepository.GetAllWithUserAsync(ct))?.ToList() ?? new List<Driver>();
            var cars = (await _unitOfWork.GetRepo<Car, int>().GetAllAsync(false, ct))?.ToList() ?? new List<Car>();
            var ratings = (await _unitOfWork.GetRepo<Rating, int>().GetAllAsync(false, ct))?.ToList() ?? new List<Rating>();

            var users = (await _unitOfWork.GetRepo<ApplicationUser, string>().GetAllAsync(false, ct))?.ToList() ?? new List<ApplicationUser>();

            foreach (var ride in rides)
            {
                ride.Passenger = users.FirstOrDefault(u => u.Id == ride.PassengerId);

                if (ride.DriverId.HasValue)
                {
                    ride.Driver = drivers.FirstOrDefault(d => d.Id == ride.DriverId.Value);
                    if (ride.Driver != null)
                    {
                        ride.Driver.Car = cars.FirstOrDefault(c => c.Id == ride.Driver.Id);
                    }
                }

                ride.Rating = ratings.FirstOrDefault(r => r.RideId == ride.RideID || r.RideId == ride.RideID); 
            }

            return rides.OrderByDescending(r => r.RequestTime);
        }

        public async Task<Result> RateDriverAsync(RateDriverVM model, string passengerId, CancellationToken ct = default)
        {
            var ride = await _unitOfWork.GetRepo<Ride, int>().FirstOrDefaultAsync(r => r.RideID == model.RideId, true, ct);

            if (ride == null) return Result.Fail("Ride not found.");
            if (ride.PassengerId != passengerId) return Result.Fail("Unauthorized to rate this ride.", ResultKind.Forbidden);
            if (ride.Status != RideStatus.Completed) return Result.Fail("You can only rate completed rides.");

            bool alreadyRated = await _unitOfWork.GetRepo<Rating, int>().AnyAsync(r => r.RideId == model.RideId, ct);
            if (alreadyRated) return Result.Fail("You have already rated this ride.");

            var rating = new Rating
            {
                RideId = model.RideId,
                PassengerId = passengerId,
                DriverId = ride.DriverId.Value,
                Score = model.Score,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.GetRepo<Rating, int>().AddAsync(rating);
            var saved = await _unitOfWork.SaveChangesAsync();

            return saved > 0 ? Result.OK() : Result.Fail("Failed to submit rating.");
        }
    }
}