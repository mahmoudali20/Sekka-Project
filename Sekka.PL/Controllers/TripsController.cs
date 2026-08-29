using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.TripVM;
using System.Security.Claims;

namespace Sekka.PL.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {

        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rides = await _tripService.GetAllRidesAsync();

            return View(rides);
        }

        [HttpGet]
        [Authorize(Roles = "Passenger")]
        public IActionResult Book()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Passenger")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookRideVM brv)
        {
            if (!ModelState.IsValid)
                return View(brv);

            brv.PassengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.BookRideAsync(brv);

            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return View(brv);
            }

            TempData["SuccessMessage"] = "Ride requested successfully! Waiting for a driver...";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Passenger")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int rideId, string reason)
        {
            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.CancelRideAsync(rideId, passengerId!, reason);

            if (!result.success)
                TempData["ErrorMessage"] = result.error;

            else
                TempData["SuccessMessage"] = "Ride cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int rideId)
        {
            var driverUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.AcceptRideAsync(rideId, driverUserId!);

            if (!result.success)
                TempData["ErrorMessage"] = result.error;

            else
                TempData["SuccessMessage"] = "Ride accepted! Head to the pickup location.";

            return RedirectToAction(
                nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int rideId)
        {
            var driverUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.StartRideAsync(rideId, driverUserId!);

            if (!result.success)
                TempData["ErrorMessage"] = result.error;

            else
                TempData["SuccessMessage"] = "Ride started! Have a safe trip.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Driver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int rideId, decimal actualFare)
        {
            var driverUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.CompleteRideAsync(rideId, driverUserId!, actualFare);

            if (!result.success)

                TempData["ErrorMessage"] = result.error;

            else
                TempData["SuccessMessage"] = "Ride completed successfully!";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Passenger")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(RateDriverVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid rating data.";
                return RedirectToAction(nameof(Index));
            }

            var passengerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _tripService.RateDriverAsync(model, passengerId!);


            if (!result.success)
                TempData["ErrorMessage"] = result.error;

            else
                TempData["SuccessMessage"] = "Thank you! Your rating has been submitted.";

            return RedirectToAction(nameof(Index));
        }
    }
}

