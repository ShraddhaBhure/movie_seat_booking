using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace movie_seat_booking.Models
{
    //public class ApplicationDbContext : DbContext
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :      base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Seat> Seat{ get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<RowGroup> RowGroups { get; set; }

        // Configure relationships in the OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of configuring relationships explicitly (if needed)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);

            // Configure the relationships for the Seat entity
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Movie)  // Seat has a foreign key to Movie
                .WithMany(m => m.Seat)  // A Movie can have many Seats
                .HasForeignKey(s => s.MovieId);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.RowGroup)  // Seat has a foreign key to RowGroup
                .WithMany(rg => rg.Seats)  // A RowGroup can have many Seats
                .HasForeignKey(s => s.RowGroupId);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Booking)  // Seat has a foreign key to Booking
                .WithMany(b => b.BookedSeats)  // A Booking can have many BookedSeats
                .HasForeignKey(s => s.BookingId);

            // Configure the relationship between Booking and Movie
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Movie)  // Booking has a foreign key to Movie
                .WithMany()  // A Movie can have many Bookings (no navigation property on Movie)
                .HasForeignKey(b => b.MovieId);
        }
    }
}
