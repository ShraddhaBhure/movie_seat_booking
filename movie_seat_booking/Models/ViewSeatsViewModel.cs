namespace movie_seat_booking.Models
{
    public class ViewSeatsViewModel
    {
        public Movie Movie { get; set; }
        public RowGroup RowGroup { get; set; }
        public List<Seat> Seats { get; set; }
        public ICollection<RowGroup> RowGroups { get; set; }  // List of RowGroups with their Seats

    }
}
