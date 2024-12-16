namespace movie_seat_booking.Models
{
    public class BookingConfirmationViewModel
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string MovieTitle { get; set; }
        public DateTime BookingTime { get; set; }
   
        public decimal BookedPrice { get; set; }
        public string PaymentStatus { get; set; }
        public List<SeatDetail> Seats { get; set; }
    }
}
