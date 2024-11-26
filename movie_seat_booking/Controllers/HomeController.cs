using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;
using System.Diagnostics;
using System.Text;

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


        public async Task<IActionResult> Index(int page = 1)
        {
            const int reviewsPerPage = 5;

            // Fetching the latest 4 movies for the banner carousel
            var latestMovies = _context.Movies
                                       .Where(m => !string.IsNullOrEmpty(m.PosterImage))  // Ensure there's a poster image
                                       .OrderByDescending(m => m.ReleaseDate)  // Order by ReleaseDate, most recent first
                                       .Take(4)  // Get the latest 4 movies
                                       .ToList();

            // Fetching movies with their average rating and review count for pagination
            var movies = await _context.Movies
                                       .Include(m => m.Ratings)
                                       .Include(m => m.Reviews)
                                       .Select(m => new MovieViewModel
                                       {
                                           MovieId = m.MovieId,
                                           Title = m.Title,
                                           CoverImage = m.CoverImage,
                                           ShowTime = m.ShowTime,
                                           Reviews = m.Reviews.OrderByDescending(r => r.ReviewDate).Skip((page - 1) * reviewsPerPage).Take(reviewsPerPage).ToList(),
                                           TotalReviews = m.Reviews.Count,
                                           AverageRating = m.Ratings.Any() ? m.Ratings.Average(r => r.Score) : 0,
                                           RatingCount = m.Ratings.Count
                                       })
                                       .ToListAsync();

            // Calculate TotalPages based on reviews per movie
            var totalPages = movies.Any()
                ? (int)Math.Ceiling(movies.First().TotalReviews / (double)reviewsPerPage)
                : 1;

            // Creating the view model for pagination and carousel
            var viewModel = new MovieCarouselViewModel
            {
                LatestMovies = latestMovies,
                MoviesIndex = new MoviesIndexViewModel
                {
                    Movies = movies,
                    CurrentPage = page,
                    TotalPages = totalPages
                }
            };

            return View(viewModel);  // Passing the combined view model to the view
        }


        public async Task<IActionResult> AllMovies(int page = 1)
        {
            const int reviewsPerPage = 5;

            var movies = await _context.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews)
                .Select(m => new MovieViewModel
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    CoverImage = m.CoverImage,
                    ShowTime = m.ShowTime,
                    Reviews = m.Reviews.OrderByDescending(r => r.ReviewDate).Skip((page - 1) * reviewsPerPage).Take(reviewsPerPage).ToList(),
                    TotalReviews = m.Reviews.Count,
                    AverageRating = m.Ratings.Any() ? m.Ratings.Average(r => r.Score) : 0,
                    RatingCount = m.Ratings.Count
                })
                .ToListAsync();

            // Handle pagination per movie
            var viewModel = new MoviesIndexViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = movies.Any()
                    ? (int)Math.Ceiling(movies.First().TotalReviews / (double)reviewsPerPage)
                    : 1 // If no reviews exist for the movie, set total pages to 1
            };

            return View(viewModel);
        }











        public IActionResult Banner()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //// GET: Movie/Index
        //public async Task<IActionResult> AllMovies(int page = 1)
        //{
        //     // Number of reviews per page
        //        const int reviewsPerPage = 5;

        //        // Fetching movies with their average rating and review count
        //        var movies = await _context.Movies
        //            .Include(m => m.Ratings)
        //            .Include(m => m.Reviews)
        //            .Select(m => new MovieViewModel
        //            {
        //                MovieId = m.MovieId,
        //                Title = m.Title,
        //                CoverImage = m.CoverImage,
        //                ShowTime = m.ShowTime,
        //                Reviews = m.Reviews.OrderByDescending(r => r.ReviewDate).Skip((page - 1) * reviewsPerPage).Take(reviewsPerPage).ToList(),
        //                TotalReviews = m.Reviews.Count,
        //                AverageRating = m.Ratings.Any() ? m.Ratings.Average(r => r.Score) : 0,
        //                RatingCount = m.Ratings.Count
        //            })
        //            .ToListAsync();

        //    //// Pass data to the view including pagination details
        //    //var viewModel = new MoviesIndexViewModel
        //    //{
        //    //    Movies = movies,
        //    //    CurrentPage = page,
        //    //    TotalPages = (int)Math.Ceiling(movies.FirstOrDefault()?.TotalReviews / (double)reviewsPerPage) // Adjusted to total reviews of all movies, you can refine this logic.
        //    //};
        //    // Ensure TotalReviews is always a non-nullable integer
        //    var totalReviews = movies.Sum(m => m.TotalReviews); // Sum of all reviews for all movies
        //    var totalReviewsForPagination = totalReviews > 0 ? totalReviews : 1; // Avoid division by zero if no reviews

        //    var viewModel = new MoviesIndexViewModel
        //    {
        //        Movies = movies,
        //        CurrentPage = page,
        //        // Calculate TotalPages, ensuring TotalReviews is a valid integer
        //        TotalPages = (int)Math.Ceiling((double)totalReviewsForPagination / reviewsPerPage)
        //    };
        //    return View(viewModel);
        //    }

    }
}
