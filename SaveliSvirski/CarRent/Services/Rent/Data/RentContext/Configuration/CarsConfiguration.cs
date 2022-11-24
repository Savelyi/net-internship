using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.RentContext.Configuration
{
    public class CarsConfiguration : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasData
            (
                new Car
                {
                    Id = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c")
                },
                new Car
                {
                    Id = new Guid("1e8743d3-bf6b-4bfd-8b74-3d523c8042cd")
                },
                new Car
                {
                    Id = new Guid("f8de7862-2ff1-4cd0-81fd-e9d34f665280")
                },
                new Car
                {
                    Id = new Guid("0692cb3a-77c3-4848-bd7d-41efd06e0884")
                },
                new Car
                {
                    Id = new Guid("3daed40f-c48c-45a5-80a7-fb1f9f1e6ff4")
                },
                new Car
                {
                    Id = new Guid("bb6a500f-eefe-465a-9793-37e368db1636")
                }
            );
        }
    }
}