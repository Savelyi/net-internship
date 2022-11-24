using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.CatalogContext.Configuration
{
    public class CarMakeConfiguration : IEntityTypeConfiguration<CarMake>
    {
        public void Configure(EntityTypeBuilder<CarMake> builder)
        {
            builder
                .HasKey(x => x.Id);
            builder.HasIndex(x => x.Make)
                .IsUnique();

            builder.HasData
            (
                new CarMake
                {
                    Id = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"),
                    Make = "BMW"
                },
                new CarMake
                {
                    Id = new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9"),
                    Make = "Mercedes"
                },
                new CarMake
                {
                    Id = new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"),
                    Make = "Audi"
                },
                new CarMake
                {
                    Id = new Guid("4b108fbc-29f8-421d-b029-195d26fa65ae"),
                    Make = "Volkswagen"
                }
            );
        }
    }
}