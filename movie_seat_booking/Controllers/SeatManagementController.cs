using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;

namespace movie_seat_booking.Controllers
{
    public class SeatManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
      
        public SeatManagementController(ApplicationDbContext context)
        {

            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        // Step 1: Show Movie Dropdown
        public IActionResult CreateSeats()
        {
            ViewData["Movies"] = new SelectList(_context.Movies, "MovieId", "Title");
            return View();
        }
        // Step 2: Fetch RowGroups based on selected MovieId
        [HttpPost]
        public IActionResult GetRowGroups(int movieId)
        {
            var rowGroups = _context.RowGroups.Where(r => r.MovieId == movieId).ToList();
            return Json(rowGroups);
        }

        // Step 3: Show Seat Layout and Allocate Seats
        [HttpPost]
        public IActionResult CreateSeatsForMovie(int movieId, int rowGroupId, int numRows, int numColumns)
        {
            var rowGroup = _context.RowGroups.FirstOrDefault(rg => rg.RowGroupId == rowGroupId);
            var seats = new List<Seat>();

            for (int row = 1; row <= numRows; row++)
            {
                for (int column = 1; column <= numColumns; column++)
                {
                    var seat = new Seat
                    {
                        RowName = row,
                        ColumnName = column,
                        IsBooked = false,  // Initially, seats are not booked
                        MovieId = movieId,
                        RowGroupId = rowGroupId,
                    };

                    seats.Add(seat);
                }
            }

            _context.Seat.AddRange(seats);
            _context.SaveChanges();

            return RedirectToAction("SeatManagement"); // Redirect to a page to manage seats
        }

        // AdminController.cs

        public IActionResult ViewSeats(int movieId, int rowGroupId)
        {
            // Fetch movie details
            var movie = _context.Movies.Find(movieId);

            // Fetch row group details
            var rowGroup = _context.RowGroups.FirstOrDefault(rg => rg.RowGroupId == rowGroupId);

            if (movie == null || rowGroup == null)
            {
                return NotFound();
            }

            // Fetch the seats for the selected movie and row group
            var seats = _context.Seat
                                .Where(s => s.MovieId == movieId && s.RowGroupId == rowGroupId)
                                .OrderBy(s => s.RowName)
                                .ThenBy(s => s.ColumnName)
                                .ToList();

            // Pass the data to the view
            var viewModel = new ViewSeatsViewModel
            {
                Movie = movie,
                RowGroup = rowGroup,
                Seats = seats
            };

            return View(viewModel);
        }

    }
}

