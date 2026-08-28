using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sekka.DAL.Models;

namespace Sekka.DAL.Configurations
{
	public class ComplaintConfig : IEntityTypeConfiguration<Complaint>
	{
		public void Configure(EntityTypeBuilder<Complaint> builder)
		{
			builder.HasIndex(c => new { c.TripId, c.Category })
				   .IsUnique()
				   .HasFilter("[TripId] IS NOT NULL")
				   .HasDatabaseName("UX_Complaints_TripId_Category");
		}
	}
}