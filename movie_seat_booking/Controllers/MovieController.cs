using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;

namespace movie_seat_booking.Controllers
{
    public class MovieController : Controller
    {
      
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View available movies
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }

        // View details of a movie and available seats
        //public IActionResult BookSeats(int movieId)
        //{
        //    var movie = _context.Movies
        //                        .Include(m => m.Seats)
        //                        .FirstOrDefault(m => m.MovieId == movieId);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    // Filter out booked seats
        //    var availableSeats = movie.Seats.Where(s => !s.IsBooked).ToList();

        //    var viewModel = new BookSeatsViewModel
        //    {
        //        Movie = movie,
        //        AvailableSeats = availableSeats
        //    };

        //    return View(viewModel);
        //}
        public IActionResult BookSeats(int movieId)
        {
            var movie = _context.Movies
                                .Include(m => m.Seat)
                                .FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Filter out the seats that are already booked
            var availableSeats = movie.Seat.Where(s => !s.IsBooked).ToList();

            return View(new BookSeatsViewModel
            {
                Movie = movie,
                AvailableSeats = availableSeats
            });
        }


        //// Handle seat booking
        //[HttpPost]
        //public IActionResult ConfirmBooking(int movieId, List<int> selectedSeatIds, string customerName)
        //{
        //    var movie = _context.Movies.Include(m => m.Seats).FirstOrDefault(m => m.MovieId == movieId);
        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    var selectedSeats = movie.Seats.Where(s => selectedSeatIds.Contains(s.SeatId) && !s.IsBooked).ToList();
        //    if (selectedSeats.Count != selectedSeatIds.Count)
        //    {
        //        // Some of the selected seats may already be booked.
        //        return BadRequest("Some selected seats are already booked.");
        //    }

        //    foreach (var seat in selectedSeats)
        //    {
        //        seat.IsBooked = true;
        //    }

        //    var booking = new Booking
        //    {
        //        MovieId = movieId,
        //        BookingTime = DateTime.Now,
        //        CustomerName = customerName,
        //        BookedSeats = selectedSeats
        //    };

        //    _context.Bookings.Add(booking);
        //    _context.SaveChanges();

        //    return RedirectToAction("BookingConfirmation", new { bookingId = booking.BookingId });
        //}


        //[HttpPost]
        //public IActionResult ConfirmBooking(int movieId, string selectedSeatIds, string customerName)
        //{
        //    var movie = _context.Movies
        //                        .Include(m => m.Seat)
        //                        .FirstOrDefault(m => m.MovieId == movieId);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    // Split the selected seat IDs and book the seats
        //    var seatIds = selectedSeatIds.Split(',').Select(int.Parse).ToList();

        //    var seatsToBook = movie.Seat.Where(s => seatIds.Contains(s.SeatId)).ToList();

        //    foreach (var seat in seatsToBook)
        //    {
        //        seat.IsBooked = true;  // Mark seat as booked
        //    }

        //    _context.SaveChanges();

        //    // You can now save the booking info in the database, e.g., in a Booking table.

        //    return RedirectToAction("BookingConfirmation");  // Redirect to a confirmation page
        //}


        //[HttpPost]
        //public IActionResult ConfirmBooking(int movieId, string selectedSeatIds, string customerName)
        //{
        //    var movie = _context.Movies
        //                        .Include(m => m.Seat)
        //                        .FirstOrDefault(m => m.MovieId == movieId);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    // Split the selected seat IDs and book the seats
        //    var seatIds = selectedSeatIds.Split(',').Select(int.Parse).ToList();

        //    var seatsToBook = movie.Seat.Where(s => seatIds.Contains(s.SeatId)).ToList();

        //    foreach (var seat in seatsToBook)
        //    {
        //        seat.IsBooked = true;
        //    }

        //    _context.SaveChanges();

        //    // You can now save the booking info in the database, e.g., in a Booking table.

        //    return RedirectToAction("BookingConfirmation");  // Redirect to a confirmation page
        //}

        // Booking confirmation page
        [HttpPost]
        public IActionResult ConfirmBooking(int movieId, string selectedSeatIds, string customerName)
        {
            var movie = _context.Movies
                                .Include(m => m.Seat)
                                .FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Split the selected seat IDs and book the seats
            var seatIds = selectedSeatIds.Split(',').Select(int.Parse).ToList();
            var seatsToBook = movie.Seat.Where(s => seatIds.Contains(s.SeatId)).ToList();

            foreach (var seat in seatsToBook)
            {
                seat.IsBooked = true;  // Mark seat as booked
            }

            // Create a booking record (assuming you have a Booking model)
            var booking = new Booking
            {
                CustomerName = customerName,
                MovieId = movie.MovieId,
                BookedSeats = seatsToBook
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Redirect to BookingConfirmation action with the booking ID
            return RedirectToAction("BookingConfirmation", new { bookingId = booking.BookingId });
        }

        // Action to display the booking confirmation page
        public IActionResult BookingConfirmation(int bookingId)
        {
            var booking = _context.Bookings
                                   .Include(b => b.BookedSeats)  // Include booked seats
                                   .ThenInclude(bs => bs.Movie)  // Include the Movie data for each seat (optional)
                                   .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

    }
}
