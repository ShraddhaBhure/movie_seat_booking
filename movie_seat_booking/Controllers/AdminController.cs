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

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
       
        // GET: Movie
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
        public async Task<IActionResult> Create(Movie movie, IFormFile coverImage)
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

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile coverImage)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
                {
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        // Delete old cover image if exists
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", movie.CoverImage);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                        // Save the new cover image
                        var fileName = $"{movie.MovieId}.jpg";
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "movieimages", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await coverImage.CopyToAsync(stream);
                        }

                        movie.CoverImage = fileName;
                    }

                    _context.Update(movie);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
          //  }
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
