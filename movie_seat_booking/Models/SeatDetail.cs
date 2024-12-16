namespace movie_seat_booking.Models
{
    public class SeatDetail
    {
        public int SeatId { get; set; }
        public string RowName { get; set; }
        public string ColumnName { get; set; }
        public decimal SeatPrice { get; set; }
    }
}
