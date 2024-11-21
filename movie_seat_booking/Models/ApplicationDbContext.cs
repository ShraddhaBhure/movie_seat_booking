using Microsoft.EntityFrameworkCore;

namespace movie_seat_booking.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Seat> Seat{ get; set; }
        public DbSet<Booking> Bookings { get; set; }
        // Configure relationships in the OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship between Movie and Seat
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Seat)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
