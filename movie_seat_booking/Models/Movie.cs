namespace movie_seat_booking.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string CoverImage { get; set; }
        public string Genre { get; set; }
        public DateTime ShowTime { get; set; }
        public decimal Price { get; set; }

        // Navigation property for related seats
        public List<Seat> Seat { get; set; }

    }
}
