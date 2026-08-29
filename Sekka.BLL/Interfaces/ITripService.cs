using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.TripVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
	public interface ITripService
	{
		Task<Result<Ride>> BookRideAsync(BookRideVM model, CancellationToken ct = default);
		Task<Result> CancelRideAsync(int rideId, string passengerId, string reason, CancellationToken ct = default);
		Task<Result> AcceptRideAsync(int rideId, string driverUserId, CancellationToken ct = default);
		Task<Result> StartRideAsync(int rideId, string driverUserId, CancellationToken ct = default);
		Task<Result> CompleteRideAsync(int rideId, string driverUserId, decimal actualFare, CancellationToken ct = default);
		Task<IEnumerable<Ride>> GetAllRidesAsync(CancellationToken ct = default);
		Task<Result> RateDriverAsync(RateDriverVM model, string passengerId, CancellationToken ct = default);

		Task<IEnumerable<Ride>> GetByPassengerAsync(string passengerId, CancellationToken ct = default);
		Task<Ride?> GetRideByIdAsync(int rideId, CancellationToken ct = default);
	}
}