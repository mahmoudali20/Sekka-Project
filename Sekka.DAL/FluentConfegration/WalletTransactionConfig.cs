using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.FluentConfegration
{
    public class WalletTransactionConfig : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Wallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Ride)
                   .WithMany()
                   .HasForeignKey(t => t.RideId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.Amount).HasPrecision(18, 2);
            builder.Property(t => t.BalanceAfter).HasPrecision(18, 2);
        }
    }
}
