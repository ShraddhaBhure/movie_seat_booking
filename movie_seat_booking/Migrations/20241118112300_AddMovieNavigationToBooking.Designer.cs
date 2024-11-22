﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using movie_seat_booking.Models;

#nullable disable

namespace movie_seat_booking.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241118112300_AddMovieNavigationToBooking")]
    partial class AddMovieNavigationToBooking
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("movie_seat_booking.Models.Booking", b =>
                {
                    b.Property<int>("BookingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BookingId"), 1L, 1);

                    b.Property<DateTime>("BookingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.HasKey("BookingId");

                    b.HasIndex("MovieId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Movie", b =>
                {
                    b.Property<int>("MovieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MovieId"), 1L, 1);

                    b.Property<string>("Genre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("ShowTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MovieId");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Seat", b =>
                {
                    b.Property<int>("SeatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SeatId"), 1L, 1);

                    b.Property<int?>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("ColumnName")
                        .HasColumnType("int");

                    b.Property<bool>("IsBooked")
                        .HasColumnType("bit");

                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("RowName")
                        .HasColumnType("int");

                    b.HasKey("SeatId");

                    b.HasIndex("BookingId");

                    b.HasIndex("MovieId");

                    b.ToTable("Seat");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Booking", b =>
                {
                    b.HasOne("movie_seat_booking.Models.Movie", "Movie")
                        .WithMany()
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Seat", b =>
                {
                    b.HasOne("movie_seat_booking.Models.Booking", null)
                        .WithMany("BookedSeats")
                        .HasForeignKey("BookingId");

                    b.HasOne("movie_seat_booking.Models.Movie", "Movie")
                        .WithMany("Seat")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Booking", b =>
                {
                    b.Navigation("BookedSeats");
                });

            modelBuilder.Entity("movie_seat_booking.Models.Movie", b =>
                {
                    b.Navigation("Seat");
                });
#pragma warning restore 612, 618
        }
    }
}