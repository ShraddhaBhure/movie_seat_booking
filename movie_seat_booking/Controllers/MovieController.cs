using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;
using movie_seat_booking.Services;
using Newtonsoft.Json;
using System.Security.Claims;

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

        // GET: Movie/Index
        public async Task<IActionResult> Index()
        {
            // Fetch movies with their ratings and reviews
            var movies = await _context.Movies
                                       .Include(m => m.Ratings)    // Include Ratings
                                       .Include(m => m.Reviews)    // Include Reviews
                                       .ToListAsync();             // Asynchronously fetch all movies

            return View(movies); // Pass the list of movies to the view
        }


        //public IActionResult Index()
        //{
        //    //var movies = _context.Movies.Include(m => m.Ratings).ToList();

        //    // Fetch movies with their ratings and reviews
        //    var movies =  _context.Movies
        //                               .Include(m => m.Ratings)    // Include Ratings
        //                               .Include(m => m.Reviews)    // Include Reviews
        //                               .ToListAsync();             // Fetch all movies
        //    return View(movies);
        //}

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

        // Search Action Method
        public async Task<IActionResult> Search(string query)
        {
            // Start with the base query for movies
            var moviesQuery = _context.Movies.AsQueryable();

            // Apply filters if the user has provided a search query
            if (!string.IsNullOrEmpty(query))
            {
                moviesQuery = moviesQuery.Where(m =>
                    m.Title.Contains(query) ||  // Search in Title
                    m.Cast.Contains(query) ||   // Search in Cast
                    m.Genre.Contains(query));   // Search in Genre
            }

            // Execute the query and fetch the results
            var movies = await moviesQuery.ToListAsync();

            // Return the search results to the view
            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies.Include(m => m.Ratings)
                                              .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Calculate the average rating
            var averageRating = movie.Ratings.Any() ? movie.Ratings.Average(r => r.Score) : 0;

            ViewBag.AverageRating = averageRating;  // Pass to view

            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitRating(int movieId, decimal score)
        {
            var movie = await _context.Movies.Include(m => m.Ratings).FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Get the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the user has already rated the movie
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);

            if (existingRating != null)
            {
                // Update the existing rating
                existingRating.Score = score;
                _context.Ratings.Update(existingRating);
            }
            else
            {
                // Create a new rating
                var rating = new Rating
                {
                    MovieId = movieId,
                    Score = score,
                    UserId = userId
                };
                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();

            return Ok(); // Return a success response
        }

     
        [HttpPost]
        public IActionResult SubmitReview(int movieId, string reviewText)
        {
            if (string.IsNullOrEmpty(reviewText))
            {
                return BadRequest("Review cannot be empty.");
            }

            // Assume you have a Review model and your movie is fetched by its ID
            var movie = _context.Movies.Include(m => m.Reviews).FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                UserId = User.Identity.Name,  // Or get user from session
                ReviewText = reviewText,
                ReviewDate = DateTime.Now,
                MovieId = movieId
            };

            movie.Reviews.Add(review);
            _context.SaveChanges();

            return Ok(); // or return the updated list of reviews
        }

      

    }
}