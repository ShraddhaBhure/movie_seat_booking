using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using movie_seat_booking.Models;
using System;

namespace movie_seat_booking.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

       
        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger)
        {
            _context = context; _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movie

        public IActionResult BannerIndex()
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

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile coverImage, IFormFile posterImage)
        {
            //if (ModelState.IsValid)
            //{
                if (coverImage != null && coverImage.Length > 0)
                {
                    // Save the cover image in wwwroot/movieimages folder
                    var fileName = $"{movie.MovieId}.jpg"; // Save image with MovieId as the name
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(stream);
                    }

                    // Assign the file name to the movie object
                    movie.CoverImage = fileName;
                }
           
            if (posterImage != null && posterImage.Length > 0)
            {
                // Save the cover image in wwwroot/movieimages folder
                var fileName = $"{movie.MovieId}.jpg"; // Save image with MovieId as the name
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieposters", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await posterImage.CopyToAsync(stream);
                }

                // Assign the file name to the movie object
                movie.PosterImage = fileName;
            }
            _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           // }
            return View(movie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Movie movie, IFormFile coverImage, IFormFile posterImage)
        //{
        //    if (id != movie.MovieId)
        //    {
        //        return NotFound();
        //    }

        //    //if (ModelState.IsValid)
        //    //{
        //     try
        //        {
        //            // Handle cover image upload
        //            if (coverImage != null && coverImage.Length > 0)
        //            {
        //                // Delete old cover image if exists
        //                if (!string.IsNullOrEmpty(movie.CoverImage))
        //                {
        //                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", movie.CoverImage);
        //                    if (System.IO.File.Exists(oldFilePath))
        //                    {
        //                        _logger.LogInformation($"Deleting old cover image: {oldFilePath}");
        //                        System.IO.File.Delete(oldFilePath);
        //                    }
        //                    else
        //                    {
        //                        _logger.LogWarning($"Cover image not found: {oldFilePath}");
        //                    }
        //                }

        //                // Save the new cover image
        //                var fileName = $"{movie.MovieId}_cover{Path.GetExtension(coverImage.FileName)}";
        //                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", fileName);

        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await coverImage.CopyToAsync(stream);
        //                }

        //                // Update the movie's cover image file name in the database
        //                movie.CoverImage = fileName;
        //            }

        //            // Handle poster image upload (optional)
        //            if (posterImage != null && posterImage.Length > 0)
        //            {
        //                var posterFileName = $"{movie.MovieId}_poster{Path.GetExtension(posterImage.FileName)}";
        //                var posterFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieposters", posterFileName);

        //                using (var stream = new FileStream(posterFilePath, FileMode.Create))
        //                {
        //                    await posterImage.CopyToAsync(stream);
        //                }

        //                movie.PosterImage = posterFileName;
        //            }

        //            // Update the movie record in the database
        //            _context.Update(movie);
        //            await _context.SaveChangesAsync();

        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!_context.Movies.Any(m => m.MovieId == movie.MovieId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    //}

        //    return View(movie);
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile coverImage, IFormFile posterImage)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            try
            {
                // Handle cover image upload (optional)
                if (coverImage != null && coverImage.Length > 0)
                {
                    // Delete old cover image if exists
                    if (!string.IsNullOrEmpty(movie.CoverImage))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", movie.CoverImage);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            _logger.LogInformation($"Deleting old cover image: {oldFilePath}");
                            System.IO.File.Delete(oldFilePath);
                        }
                        else
                        {
                            _logger.LogWarning($"Cover image not found: {oldFilePath}");
                        }
                    }

                    // Save the new cover image
                    var fileName = $"{movie.MovieId}_cover{Path.GetExtension(coverImage.FileName)}";
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(stream);
                    }

                    // Update the movie's cover image file name in the database
                    movie.CoverImage = fileName;
                }

                // Handle poster image upload (optional)
                if (posterImage != null && posterImage.Length > 0)
                {
                    // Save the new poster image
                    var posterFileName = $"{movie.MovieId}_poster{Path.GetExtension(posterImage.FileName)}";
                    var posterFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieposters", posterFileName);

                    using (var stream = new FileStream(posterFilePath, FileMode.Create))
                    {
                        await posterImage.CopyToAsync(stream);
                    }

                    // Update the movie's poster image file name in the database
                    movie.PosterImage = posterFileName;
                }
                // If no poster image is provided, retain the original value of PosterImage

                // Update the movie record in the database
                _context.Update(movie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(m => m.MovieId == movie.MovieId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return View(movie);
        }


        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                // Delete the movie cover image from disk
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", movie.CoverImage);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }





    }
}
