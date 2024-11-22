namespace movie_seat_booking.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string CoverImage { get; set; }
        public string PosterImage { get; set; }
        public string Genre { get; set; }
        public DateTime ShowTime { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }
        public string Cast { get; set; }
        public string Trailer { get; set; }
        public string Plot { get; set; }

        // Navigation property for related seats

      //  public List<Rating> Ratings { get; set; }   // A movie can have many ratings
        public List<Review> Reviews { get; set; }
        public List<Seat> Seat { get; set; }
        public ICollection<Rating> Ratings { get; set; } // Typically ICollection<T> is used for navigation properties


    }
}
