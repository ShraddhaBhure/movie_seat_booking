using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace movie_seat_booking.Models
{
    //public class ApplicationDbContext : DbContext
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Seat> Seat{ get; set; }
        public DbSet<Booking> Bookings { get; set; }
        // Configure relationships in the OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Configure one-to-many relationship between Movie and Seat
            //modelBuilder.Entity<Movie>()
            //    .HasMany(m => m.Seat)
            //    .WithOne(s => s.Movie)
            //    .HasForeignKey(s => s.MovieId);

            //base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Seat)
                .HasForeignKey(s => s.MovieId);
        }
    }
}
