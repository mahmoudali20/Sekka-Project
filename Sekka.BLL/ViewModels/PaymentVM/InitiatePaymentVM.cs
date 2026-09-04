using Sekka.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.PaymentVM
{
    public class InitiatePaymentVM
    {
        [Required]
        public int RideId { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }
    }

    public class PaymentVM
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Message { get; set; }

        // l kashier 3shan el external payment w kda
        public string? RedirectUrl { get; set; }
    }
}
