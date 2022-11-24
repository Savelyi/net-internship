﻿// <auto-generated />
using System;
using Data.RentContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(RentDbContext))]
    [Migration("20220811120312_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Data.Models.Car", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Cars");

                    b.HasData(
                        new
                        {
                            Id = new Guid("eff31885-c7d4-4f88-8559-4a676fdc5a5c"),
                            IsAvailable = true
                        },
                        new
                        {
                            Id = new Guid("1e8743d3-bf6b-4bfd-8b74-3d523c8042cd"),
                            IsAvailable = true
                        },
                        new
                        {
                            Id = new Guid("f8de7862-2ff1-4cd0-81fd-e9d34f665280"),
                            IsAvailable = true
                        },
                        new
                        {
                            Id = new Guid("0692cb3a-77c3-4848-bd7d-41efd06e0884"),
                            IsAvailable = true
                        },
                        new
                        {
                            Id = new Guid("3daed40f-c48c-45a5-80a7-fb1f9f1e6ff4"),
                            IsAvailable = true
                        },
                        new
                        {
                            Id = new Guid("bb6a500f-eefe-465a-9793-37e368db1636"),
                            IsAvailable = true
                        });
                });

            modelBuilder.Entity("Data.Models.Rent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CarId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Closed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CarId");

                    b.ToTable("Rents");
                });

            modelBuilder.Entity("Data.Models.Rent", b =>
                {
                    b.HasOne("Data.Models.Car", "CarInfo")
                        .WithMany("RentInfo")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("CarInfo");
                });

            modelBuilder.Entity("Data.Models.Car", b =>
                {
                    b.Navigation("RentInfo");
                });
#pragma warning restore 612, 618
        }
    }
}