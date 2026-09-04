using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Ride)
                   .WithMany()
                   .HasForeignKey(p => p.RideId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Coupon)
                   .WithMany()
                   .HasForeignKey(p => p.CouponId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Amount).HasPrecision(18, 2);

            // One payment per ride.
            builder.HasIndex(p => p.RideId).IsUnique();
        }
    }
}
