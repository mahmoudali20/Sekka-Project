using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class CouponConfig : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.DiscountAmount).HasPrecision(18, 2);

            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
