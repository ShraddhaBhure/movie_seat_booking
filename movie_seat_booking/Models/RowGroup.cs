namespace movie_seat_booking.Models
{
    public class RowGroup
    {
        public int RowGroupId { get; set; }
        public string GroupName { get; set; } // "Front", "Middle", "Back"
        public decimal Price { get; set; }  
       

        public int MovieId { get; set; }

        public Movie Movie { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
