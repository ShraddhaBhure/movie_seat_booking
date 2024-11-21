namespace movie_seat_booking.Models
{
    public class BookSeatsViewModel
    {
        public Movie Movie { get; set; }
        public List<Seat> AvailableSeats { get; set; }
        public Booking Booking { get; set; }       //////newly

    }

}
