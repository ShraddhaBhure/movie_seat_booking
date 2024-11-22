namespace movie_seat_booking.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int MovieId { get; set; }  // Foreign key to Movie
        public decimal Score { get; set; }  // Rating score, typically between 1-5 or 1-10
        public string UserId { get; set; }  // User who rated the movie (optional, could be nullable)

        // Navigation property
        public Movie Movie { get; set; }
        public ApplicationUser User { get; set; }
    }
}
