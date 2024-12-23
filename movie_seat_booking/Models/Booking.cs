﻿namespace movie_seat_booking.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int MovieId { get; set; }
        public DateTime BookingTime { get; set; }
        public List<Seat> BookedSeats { get; set; }
        public string CustomerName { get; set; }

        public string CustomerId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal BookedPrice { get; set; }

     
        public Movie Movie { get; set; }  

    }
}
