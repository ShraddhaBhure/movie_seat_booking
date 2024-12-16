using Microsoft.AspNetCore.Mvc; // For MVC actions and views
using Microsoft.AspNetCore.Identity; // For Identity-related functionality (if needed for user authentication)
using System.Threading.Tasks; // For async methods
using movie_seat_booking.Models; // For your application models (e.g., Booking, etc.)
using Microsoft.AspNetCore.Authorization; // For access control (if needed)
using movie_seat_booking.Services; // For your service layer (if applicable)
using Microsoft.EntityFrameworkCore; // For Entity Framework Core (DB context, queries)
using System.Security.Claims; // For handling claims (e.g., for identifying logged-in user)
using ZXing; // For QR code generation (ZXing.Net library)
using System.Drawing; // For image manipulation (for QR code)
using System.Drawing.Imaging; // For image formats (used for saving QR code as PNG)
using iTextSharp.text; // For PDF generation (iTextSharp library)
using iTextSharp.text.pdf; // For PDF writer (iTextSharp library)
using Stripe; // For Stripe payment integration (if you're handling payments)
using QRCoder;
using ZXing.Common; // For QR Code generation (QRCoder library)
using System.IO;

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
        [HttpPost]
        public async Task<IActionResult> ProceedToPayment(int bookingId, decimal totalPrice)
        {
            var booking = await _context.Bookings.Include(b => b.Movie).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            // Update booking details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you're using ClaimsIdentity to store the user ID
            booking.PaymentStatus = "Paid";
            booking.CustomerId = userId;

            _context.Update(booking);
            await _context.SaveChangesAsync();

            // Generate PDF ticket
            var filePath = GeneratePdfTicket(booking);

            // Return the user to the download page
            return RedirectToAction("DownloadTicket", new { bookingId = booking.BookingId });
        }

        public IActionResult DownloadTicket(int bookingId)
        {
            var booking = _context.Bookings.Include(b => b.Movie).FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            // Determine the ticket's file path and file name
            var fileName = $"{booking.BookingId}_{booking.MovieId}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tickets", fileName);

            // Pass the file name to the view (not full path)
            var model = new TicketDownloadViewModel
            {
                Booking = booking,
                FilePath = filePath, // Full path to the file
                FileName = fileName  // Just the file name (for the download link)
            };

            return View(model);
        }

        private string GeneratePdfTicket(Booking booking)
        {
            var fileName = $"{booking.BookingId}_{booking.MovieId}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tickets", fileName);

            using (var memoryStream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, memoryStream);
            

                doc.Open();
                // Add ticket content
                doc.Add(new Paragraph("Booking Confirmation"));
                doc.Add(new Paragraph($"Booking ID: {booking.BookingId}"));
                doc.Add(new Paragraph($"Customer: {booking.CustomerName}"));
                doc.Add(new Paragraph($"Movie: {booking.Movie?.Title ?? "Not Available"}"));
                doc.Add(new Paragraph($"Total Price: {booking.BookedPrice:C}"));
                doc.Add(new Paragraph($"Payment Status: {booking.PaymentStatus}"));

                // Prepare a string with all the booking details for the QR code
                var qrCodeData = $"Booking ID: {booking.BookingId}\n" +
                                 $"Customer: {booking.CustomerName}\n" +
                                 $"Movie: {booking.Movie?.Title ?? "Not Available"}\n" +
                                 $"Total Price: {booking.BookedPrice:C}\n" +
                                 $"Payment Status: {booking.PaymentStatus}";

                // Generate the QR Code with all the booking details
                var qrCodeBytes = GenerateQrCode(qrCodeData);

                // Create the QR image from the byte array
                var qrImage = iTextSharp.text.Image.GetInstance(qrCodeBytes);
                qrImage.ScaleAbsolute(100, 100); // Resize the QR code if needed
                doc.Add(qrImage);

                doc.Close();

                // Save the PDF to a file
                System.IO.File.WriteAllBytes(filePath, memoryStream.ToArray());
            }

            return filePath;
        }
        private byte[] GenerateQrCode(string text)
        {
            var qrWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,  // You can adjust the height and width as needed
                    Width = 200
                }
            };

            // Generate the pixel data for the QR code
            var pixelData = qrWriter.Write(text);

            // Create a Bitmap using the pixel data
            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);

            // Convert pixel data to Bitmap
            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    // ZXing uses 0 for black and 1 for white (or vice versa depending on encoding)
                    int pixel = pixelData.Pixels[y * pixelData.Width + x];
                    Color color = pixel == 0 ? Color.Black : Color.White; // Handle black/white pixel
                    bitmap.SetPixel(x, y, color); // Set pixel on the Bitmap
                }
            }

            // Save the Bitmap to a memory stream and return it as a byte array (PNG format)
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
       

    }
}
