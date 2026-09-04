using System.ComponentModel.DataAnnotations;
using Sekka.DAL.Models;

namespace Sekka.BLL.ViewModels.TripVM
{
    public class BookRideVM
    {

        public string? PassengerId { get; set; }

        [Required(ErrorMessage = "Choose how you'll pay")]
        public PaymentMethod PreferredPaymentMethod { get; set; } = PaymentMethod.Cash;

        [Required(ErrorMessage = "Pickup Location is required")]
        public string PickupLocation { get; set; } = string.Empty;
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }

        [Required(ErrorMessage = "Dropoff Location is required")]
        public string DropoffLocation { get; set; } = string.Empty;
        public double DropoffLat { get; set; }
        public double DropoffLng { get; set; }

        public decimal EstimatedFare { get; set; }
        public double DistanceInKm { get; set; } 

        public DateTime? ScheduledTime { get; set; }
    }
}
