using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.RentContext.Configuration
{
    public class RentConfiguration : IEntityTypeConfiguration<Rent>
    {
        public void Configure(EntityTypeBuilder<Rent> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.CarInfo)
                .WithMany(e => e.RentInfo)
                .HasForeignKey(e => e.CarId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}