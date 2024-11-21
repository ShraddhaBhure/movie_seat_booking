namespace movie_seat_booking.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int MovieId { get; set; }  // Foreign key to Movie
        public string ReviewText { get; set; }  // The actual review text
        public string UserId { get; set; }  // User who wrote the review (optional, could be nullable)
        public DateTime ReviewDate { get; set; }  // Date when the review was posted

        // Navigation property
        public Movie Movie { get; set; }
    }
}
