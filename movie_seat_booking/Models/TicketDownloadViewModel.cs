namespace movie_seat_booking.Models
{
    public class TicketDownloadViewModel
    {
    
            public Booking Booking { get; set; }
            public string PaymentStatus { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; } // Full file path for download

      //  public Booking Booking { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
