using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.VehicleNumber)
                .IsUnique();
            builder.HasOne(c => c.CarMakeInfo)
                .WithMany(x => x.CarModelInfo)
                .HasForeignKey(c => c.CarMakeId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasData
            (
                new CarModel
                {
                    Id = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c"),
                    CarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"), //BMW
                    Model = "M5",
                    RentPrice = 500,
                    VehicleNumber = "1234AB-3"
                },
                new CarModel
                {
                    Id = new Guid("1e8743d3-bf6b-4bfd-8b74-3d523c8042cd"),
                    CarMakeId = new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9"), //Mercedes
                    Model = "C43",
                    RentPrice = 600,
                    VehicleNumber = "4321BC-7"
                },
                new CarModel
                {
                    Id = new Guid("f8de7862-2ff1-4cd0-81fd-e9d34f665280"),
                    CarMakeId = new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"), //Audi
                    Model = "RS4",
                    RentPrice = 550,
                    VehicleNumber = "5674RY-7"
                },
                new CarModel
                {
                    Id = new Guid("0692cb3a-77c3-4848-bd7d-41efd06e0884"),
                    CarMakeId = new Guid("4b108fbc-29f8-421d-b029-195d26fa65ae"), //Volkswagen
                    Model = "Polo",
                    RentPrice = 300,
                    VehicleNumber = "7676XD-7"
                },
                new CarModel
                {
                    Id = new Guid("3daed40f-c48c-45a5-80a7-fb1f9f1e6ff4"),
                    CarMakeId = new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"), //BMW
                    Model = "X6",
                    RentPrice = 650,
                    VehicleNumber = "5436UI-5"
                },
                new CarModel
                {
                    Id = new Guid("bb6a500f-eefe-465a-9793-37e368db1636"),
                    CarMakeId = new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"), //Audi
                    Model = "A6",
                    RentPrice = 500,
                    VehicleNumber = "9817YU-5"
                }
            );
        }
    }
}