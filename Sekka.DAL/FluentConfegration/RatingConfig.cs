using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class RatingConfig : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(r => r.Id);

            
            builder.HasOne(r => r.Driver)
                   .WithMany()
                   .HasForeignKey(r => r.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(r => r.Passenger)
                   .WithMany()
                   .HasForeignKey(r => r.PassengerId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(r => r.Ride)
                   .WithOne(r => r.Rating)
                   .HasForeignKey<Rating>(r => r.RideId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}