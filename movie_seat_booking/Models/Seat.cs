namespace movie_seat_booking.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int RowName { get; set; }
        public int ColumnName { get; set; }
        public bool IsBooked { get; set; }
        public int MovieId { get; set; }
        public int? BookingId { get; set; }

        // Reference to RowGroup
        public int RowGroupId { get; set; }
        public RowGroup RowGroup { get; set; }  // Navigation property for RowGroup

        public Movie Movie { get; set; }
        public Booking Booking { get; set; }
    }
}
