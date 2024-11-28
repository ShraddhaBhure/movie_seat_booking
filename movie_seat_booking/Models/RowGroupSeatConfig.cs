namespace movie_seat_booking.Models
{
    public class RowGroupSeatConfig
    {
        public int RowGroupId { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public decimal Price { get; set; }
    }
}
