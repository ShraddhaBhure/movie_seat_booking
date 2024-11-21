namespace movie_seat_booking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int MovieId { get; set; }
        public DateTime BookingTime { get; set; }
        public List<Seat> BookedSeats { get; set; }
        public string CustomerName { get; set; }


        public Movie Movie { get; set; }  // Add this property

    }
}
