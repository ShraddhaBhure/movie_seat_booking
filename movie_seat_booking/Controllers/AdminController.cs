using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using movie_seat_booking.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace movie_seat_booking.Controllers
{
    //[Authorize(Roles = "Admin")]
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

        // GET: Admin/ManageSeats/{movieId}
        public async Task<IActionResult> ManageSeats(int movieId)
        {
            var movie = await _context.Movies.Include(m => m.Seat)
                                              .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var model = new BookSeatsViewModel
            {
                Movie = movie,
                AvailableSeats = movie.Seat.Where(s => !s.IsBooked).ToList()
            };

            return View(model);
        }
   

        // POST: Admin/UpdateSeats/{movieId}
        [HttpPost]
        public async Task<IActionResult> UpdateSeats(int movieId, List<int> selectedSeats)
        {
            var movie = await _context.Movies.Include(m => m.Seat)
                                              .FirstOrDefaultAsync(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Get the seats that are being selected
            var seatsToUpdate = await _context.Seat
                                              .Where(s => selectedSeats.Contains(s.SeatId) && s.MovieId == movieId)
                                              .ToListAsync();

            foreach (var seat in seatsToUpdate)
            {
                seat.IsBooked = true;  // Mark the seat as booked
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ManageSeats", new { movieId = movieId });
        }


        //// Admin controller action to display the movie list and create seats form
        //public IActionResult CreateSeats()
        //{
        //    // Fetch the list of all movies from the database
        //    var movies = _context.Movies.ToList();

        //    // Return the view with the movie list
        //    return View(new CreateSeatsViewModel
        //    {
        //        Movies = movies
        //    });
        //}

        //// Admin controller action to display the movie list and create seats form
        //public IActionResult CreateSeats()
        //{
        //    // Fetch the list of all movies and row groups from the database
        //    var movies = _context.Movies
        //                         .Include(m => m.RowGroups)  // Include RowGroups for each movie
        //                         .ToList();

        //    // Return the view with the movie list and row groups
        //    return View(new CreateSeatsViewModel
        //    {
        //        Movies = movies,
        //        RowGroups = _context.RowGroups.ToList()  // Fetch available row groups
        //    });
        //}

        //// Admin controller action to create the seats for the selected movie
        //[HttpPost]
        //public IActionResult CreateSeats(CreateSeatsViewModel model)
        //{
        //    // Ensure the movie exists
        //    var movie = _context.Movies.FirstOrDefault(m => m.MovieId == model.MovieId);
        //    if (movie == null)
        //    {
        //        return NotFound();  // Return 404 if the movie doesn't exist
        //    }

        //    // Create the seats for the selected movie based on the row and column count
        //    var seats = new List<Seat>();

        //    for (int row = 1; row <= model.Rows; row++)
        //    {
        //        for (int col = 1; col <= model.Columns; col++)
        //        {
        //            seats.Add(new Seat
        //            {
        //                RowName = row,
        //                ColumnName = col,
        //                IsBooked = false,  // Seat is initially empty
        //                MovieId = model.MovieId,
        //                BookingId = null  // BookingId will be null initially
        //            });
        //        }
        //    }

        //    // Add the seats to the database
        //    _context.Seat.AddRange(seats);
        //    _context.SaveChanges();

        //    // Redirect to the movie list or a confirmation page
        //    return RedirectToAction("Index", "Admin");  // Redirect to admin movie list (or another page)
        //}

        //[HttpPost]
        //public IActionResult CreateSeats(CreateSeatsViewModel model)
        //{
        //    // Ensure the movie exists
        //    var movie = _context.Movies.FirstOrDefault(m => m.MovieId == model.MovieId);
        //    if (movie == null)
        //    {
        //        return NotFound();  // Return 404 if the movie doesn't exist
        //    }

        //    // Create the row groups if they don't exist
        //    var frontRowGroup = _context.RowGroups.FirstOrDefault(rg => rg.GroupName == "Front");
        //    var middleRowGroup = _context.RowGroups.FirstOrDefault(rg => rg.GroupName == "Middle");
        //    var backRowGroup = _context.RowGroups.FirstOrDefault(rg => rg.GroupName == "Back");

        //    // Ensure row groups exist for pricing
        //    if (frontRowGroup == null || middleRowGroup == null || backRowGroup == null)
        //    {
        //        return BadRequest("Row groups not found.");
        //    }

        //    // Create the seats for the selected movie
        //    var seats = new List<Seat>();

        //    for (int row = 1; row <= model.Rows; row++)
        //    {
        //        RowGroup rowGroup;

        //        // Determine which row group to assign based on row number
        //        if (row <= model.Rows / 3)
        //        {
        //            rowGroup = frontRowGroup;
        //        }
        //        else if (row <= 2 * (model.Rows / 3))
        //        {
        //            rowGroup = middleRowGroup;
        //        }
        //        else
        //        {
        //            rowGroup = backRowGroup;
        //        }

        //        // Create seats for each column
        //        for (int col = 1; col <= model.Columns; col++)
        //        {
        //            seats.Add(new Seat
        //            {
        //                RowName = row,
        //                ColumnName = col,
        //                IsBooked = false,  // Seat is initially empty
        //                MovieId = model.MovieId,
        //                RowGroupId = rowGroup.RowGroupId,  // Assign the row group
        //                BookingId = null  // BookingId will be null initially
        //            });
        //        }
        //    }

        //    // Add the seats to the database
        //    _context.Seat.AddRange(seats);
        //    _context.SaveChanges();

        //    // Redirect to the movie list or a confirmation page
        //    return RedirectToAction("Index", "Admin");  // Redirect to admin movie list (or another page)
        //}



        //public IActionResult ViewSeats(int movieId)
        //{
        //    // Fetch the movie along with its seats, and order them by RowName and ColumnName
        //    var movie = _context.Movies
        //                        .Include(m => m.Seat)  // Ensure seats are included
        //                        .FirstOrDefault(m => m.MovieId == movieId);

        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }

        //    // Order the seats by RowName and ColumnName before passing to the view
        //    movie.Seat = movie.Seat.OrderBy(s => s.RowName).ThenBy(s => s.ColumnName).ToList();

        //    // Return the movie along with its ordered seats to the view
        //    return View(movie);
        //}

        public IActionResult CreateSeats()
        {
            //ViewData["Movies"] = new SelectList(_context.Movies, "MovieId", "Title");
            //return View();
            var movies = _context.Movies.ToList();

            var rowgroup = _context.RowGroups.ToList();
            // Create a SelectList for the dropdown
            ViewData["Movies"] = new SelectList(movies, "MovieId", "Title");
            ViewData["RowGroup"] = new SelectList(rowgroup, "RowGroupId", "GroupName");

            // Return the view
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
            // Return the view
            //  return RedirectToAction("ViewSeats"); // Redirect to a page to manage seats
            return RedirectToAction("Index", "Admin");

        }

        public IActionResult ViewSeats(int movieId)
        {
            // Fetch the movie along with its row groups and seats
            var movie = _context.Movies
                                .Include(m => m.RowGroups)   // Include related RowGroups
                                .ThenInclude(rg => rg.Seats) // Include related Seats for RowGroups
                                .FirstOrDefault(m => m.MovieId == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            // Ensure that seats in each row group are ordered by RowName and ColumnName
            foreach (var rowGroup in movie.RowGroups)
            {
                rowGroup.Seats = rowGroup.Seats.OrderBy(s => s.RowName).ThenBy(s => s.ColumnName).ToList();
            }

            // Return the movie along with its row groups and ordered seats to the view
            var viewModel = new ViewSeatsViewModel
            {
                Movie = movie,
                RowGroups = movie.RowGroups
            };

            return View(viewModel);
        }


        //----------------------------------- GET: RowGroups----------------------
        public async Task<IActionResult> IndexRows()
        {
            var rowGroups = await _context.RowGroups.ToListAsync();
            return View(rowGroups);
        }

        // GET: RowGroups/Details/5
        public async Task<IActionResult> DetailsRows(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rowGroup = await _context.RowGroups
                .Include(rg => rg.Seats) // Optionally include seats if needed
                .FirstOrDefaultAsync(m => m.RowGroupId == id);
            if (rowGroup == null)
            {
                return NotFound();
            }

            return View(rowGroup);
        }

        // GET: RowGroups/Create
        public IActionResult CreateRows()
        {
            return View();
        }

        // POST: RowGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRows([Bind("RowGroupId,GroupName,Price")] RowGroup rowGroup)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(rowGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           // }
            return View(rowGroup);
        }

        // GET: RowGroups/Edit/5
        public async Task<IActionResult> EditRows(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rowGroup = await _context.RowGroups.FindAsync(id);
            if (rowGroup == null)
            {
                return NotFound();
            }
            return View(rowGroup);
        }

        // POST: RowGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRows(int id, [Bind("RowGroupId,GroupName,Price")] RowGroup rowGroup)
        {
            if (id != rowGroup.RowGroupId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(rowGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RowGroupExists(rowGroup.RowGroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
           // }
            return View(rowGroup);
        }

        // GET: RowGroups/Delete/5
        public async Task<IActionResult> DeleteRows(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rowGroup = await _context.RowGroups
                .FirstOrDefaultAsync(m => m.RowGroupId == id);
            if (rowGroup == null)
            {
                return NotFound();
            }

            return View(rowGroup);
        }

        // POST: RowGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedRows(int id)
        {
            var rowGroup = await _context.RowGroups.FindAsync(id);
            _context.RowGroups.Remove(rowGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RowGroupExists(int id)
        {
            return _context.RowGroups.Any(e => e.RowGroupId == id);
        }

    }
}
