using Microsoft.AspNetCore.Mvc;
using movie_seat_booking.Models;
using System.Diagnostics;

namespace movie_seat_booking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var latestMovies = _context.Movies
                                       .Where(m => !string.IsNullOrEmpty(m.PosterImage))  // Ensure there's a poster image
                                       .OrderByDescending(m => m.ReleaseDate)  // Order by ReleaseDate, most recent first
                                       .Take(4)  // Get the latest 4 movies
                                       .ToList();

            // Passing to ViewBag so that it's available in Layout
            ViewBag.LatestMovies = latestMovies;

            return View(latestMovies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
