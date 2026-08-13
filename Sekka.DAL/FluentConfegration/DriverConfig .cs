using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class DriverConfig : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);
            builder.HasOne(d => d.User)
                   .WithOne(u => u.Driver)
                   .HasForeignKey<Driver>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Car)
                   .WithOne(c => c.Driver)
                   .HasForeignKey<Driver>(d => d.CarId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(d => d.LicenseNumber)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.RatingAverage)
                   .HasPrecision(3, 2);

            builder.Property(d => d.IsAvailable)
                   .HasDefaultValue(false);
        }

    }
}
