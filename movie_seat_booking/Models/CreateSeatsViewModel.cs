namespace movie_seat_booking.Models
{
    public class CreateSeatsViewModel
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public List<Movie> Movies { get; set; }
        public List<RowGroup> RowGroups { get; set; }  // To manage different row groups for each movie

        public int Rows { get; set; }
        public int Columns { get; set; }

        public decimal FrontRowPrice { get; set; }
        public decimal MiddleRowPrice { get; set; }
        public decimal BackRowPrice { get; set; }
        public List<RowGroupSeatConfig> RowGroupSeatConfigs { get; set; }

    }
}
