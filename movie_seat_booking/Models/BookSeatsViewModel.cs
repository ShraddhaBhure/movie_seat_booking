namespace movie_seat_booking.Models
{
    public class BookSeatsViewModel
    {
        public Movie Movie { get; set; }
        public List<Seat> AvailableSeats { get; set; }
        public Booking Booking { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerName { get; set; }
        //////newly

    }

}
