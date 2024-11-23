namespace movie_seat_booking.Models
{
    public class MovieCarouselViewModel
    {
  
    public List<Movie> LatestMovies { get; set; }  // For the carousel banner
    public MoviesIndexViewModel MoviesIndex { get; set; }  // For the paginated movies with reviews and ratings

    }
}
