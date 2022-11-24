using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Makes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makes", x => x.Id);
                    table.UniqueConstraint("AK_Makes_Make", x => x.Make);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    RentPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CarMakeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.UniqueConstraint("AK_Models_VehicleNumber", x => x.VehicleNumber);
                    table.ForeignKey(
                        name: "FK_Models_Makes_CarMakeId",
                        column: x => x.CarMakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Makes",
                columns: new[] { "Id", "Make" },
                values: new object[,]
                {
                    { new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"), "BMW" },
                    { new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9"), "Mercedes" },
                    { new Guid("4b108fbc-29f8-421d-b029-195d26fa65ae"), "Volkswagen" },
                    { new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"), "Audi" }
                });

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "CarMakeId", "IsAvailable", "Model", "RentPrice", "VehicleNumber" },
                values: new object[,]
                {
                    { new Guid("0692cb3a-77c3-4848-bd7d-41efd06e0884"), new Guid("4b108fbc-29f8-421d-b029-195d26fa65ae"), true, "Polo", 300m, "7676XD-7" },
                    { new Guid("1e8743d3-bf6b-4bfd-8b74-3d523c8042cd"), new Guid("3c3dbfdc-8329-43a8-aaae-8f2049b564f9"), true, "C43", 600m, "4321BC-7" },
                    { new Guid("3daed40f-c48c-45a5-80a7-fb1f9f1e6ff4"), new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"), true, "X6", 650m, "5436UI-5" },
                    { new Guid("bb6a500f-eefe-465a-9793-37e368db1636"), new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"), true, "A6", 500m, "9817YU-5" },
                    { new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c"), new Guid("10905a00-6578-41f4-9784-edafbf19ed4b"), true, "M5", 500m, "1234AB-3" },
                    { new Guid("f8de7862-2ff1-4cd0-81fd-e9d34f665280"), new Guid("8403feae-5c00-4fab-8ccd-475602fc111e"), true, "RS4", 550m, "5674RY-7" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Models_CarMakeId",
                table: "Models",
                column: "CarMakeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Makes");
        }
    }
}
