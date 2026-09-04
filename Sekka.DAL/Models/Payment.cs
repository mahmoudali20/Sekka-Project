namespace Sekka.DAL.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public int RideId { get; set; }
        public Ride Ride { get; set; } = default!;

        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        // l kashier bs brdo
        public string? ProviderReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
