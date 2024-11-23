namespace movie_seat_booking.Models
{
    public class MovieViewModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string CoverImage { get; set; }
        public DateTime ShowTime { get; set; }
        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }
        public int TotalReviews { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
