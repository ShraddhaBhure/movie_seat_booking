using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;
using movie_seat_booking.Services;
using Newtonsoft.Json;

namespace movie_seat_booking.Controllers
{
    public class MovieController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly SmsService _smsService;
        public MovieController(ApplicationDbContext context, SmsService smsService)
        {
            _smsService = smsService;
            _context = context;
        }

        // View available movies
        public IActionResult Index()
        {
            var movies = _context.Movies.ToList();
            return View(movies);
        }
        public async Task<IActionResult> MovieDetails(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Ratings)   // Load related ratings
                .Include(m => m.Reviews)   // Load related reviews
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);  // Pass the movie object to the view
        }

        public async Task<IActionResult> AddRating(int movieId, decimal score)
        {
            var movie = await _context.Movies.FindAsync(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var rating = new Rating
            {
                MovieId = movieId,
                Score = score,
                UserId = User.Identity.Name  // Or whatever user system you're using
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("MovieDetails", new { id = movieId });
        }
        public async Task<IActionResult> AddReview(int movieId, string reviewText)
        {
            var movie = await _context.Movies.FindAsync(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                MovieId = movieId,
                ReviewText = reviewText,
                UserId = User.Identity.Name,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("MovieDetails", new { id = movieId });
        }



        // View details of a movie and available seats

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

            //Create a booking record(assuming you have a Booking model)
            var booking = new Booking
            {
                CustomerName = customerName,
                MovieId = movie.MovieId,
                BookedSeats = seatsToBook
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            ///Redirect to BookingConfirmation action with the booking ID
            return RedirectToAction("BookingConfirmation", new { bookingId = booking.BookingId });
        }

        ////// Action to display the booking confirmation page
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
   


    } }