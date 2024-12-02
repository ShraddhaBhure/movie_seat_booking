using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using movie_seat_booking.Models;
using Microsoft.AspNetCore.Authorization;
using movie_seat_booking.Services;
using Microsoft.EntityFrameworkCore;




namespace movie_seat_booking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor with dependency injection for DbContext and UserManager
        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Action to get the booking history for the logged-in user
        public async Task<IActionResult> OrderHistory()
        {
            // Declare the variable before the if statement
            List<Booking> bookings = new List<Booking>();

            if (User.Identity.IsAuthenticated)
            {
                // Get the logged-in user's ID (CustomerId)
                var userId = _userManager.GetUserId(User);  // This fetches the user's Id from AspNetUsers

                // Fetch the bookings associated with the logged-in user
                bookings = await _context.Bookings
                    .Include(b => b.Movie)   // Ensure Movie details are loaded
                    .Include(b => b.BookedSeats)  // Include the booked seats
                    .Where(b => b.CustomerId == userId)
                    .OrderByDescending(b => b.BookingTime)  // Order by booking time (latest first)
                    .ToListAsync();
            }

            // Return the data to the view, even if the user is not authenticated
            // Optionally, you could redirect to the login page if not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            return View(bookings);
        }
        // GET: PendingPayments
        public async Task<IActionResult> PendingPayments()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get the logged-in user's ID (CustomerId)
                var userId = _userManager.GetUserId(User);  // This fetches the user's Id from AspNetUsers

                // Fetch bookings that have 'NA' (Not Available) as PaymentStatus for the logged-in user
                var pendingPayments = await _context.Bookings
                    .Include(b => b.Movie)  // Ensure Movie details are loaded
                    .Where(b => b.CustomerId == userId && b.PaymentStatus == "NA")
                    .OrderByDescending(b => b.BookingTime)  // Optionally order by BookingTime
                    .ToListAsync();

                // Return the data to the view
                return View(pendingPayments);
            }

            // Redirect to login if not authenticated
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
