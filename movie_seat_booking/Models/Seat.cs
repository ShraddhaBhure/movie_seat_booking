namespace movie_seat_booking.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int RowName { get; set; }
        public int ColumnName { get; set; }
        public bool IsBooked { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
