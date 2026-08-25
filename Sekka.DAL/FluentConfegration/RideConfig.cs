using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class RideConfig: IEntityTypeConfiguration<Ride>
    {
        public void Configure(EntityTypeBuilder<Ride> builder)
        {
            builder.HasKey(r => r.RideID);

            builder.HasOne(r => r.Passenger)
                   .WithMany()
                   .HasForeignKey(r => r.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict); 

            
            builder.HasOne(r => r.Driver)
                   .WithMany()
                   .HasForeignKey(r => r.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.Property(r => r.EstimatedFare).HasPrecision(18, 2);
            builder.Property(r => r.ActualFare).HasPrecision(18, 2);

            builder.Property(r => r.PickupLocation).IsRequired();
            builder.Property(r => r.DropoffLocation).IsRequired();

        }
    }
}
