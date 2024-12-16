using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;
using Stripe;
using QRCoder;
using QRCoder.Core;
using Microsoft.AspNetCore.Identity;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;



namespace movie_seat_booking.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _stripeSecretKey;
        private readonly string _stripePublishableKey;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly TicketGenerator _ticketGenerator; // Service to generate the ticket PDF



        public PaymentController(IConfiguration configuration, /*TicketGenerator ticketGenerator,*/ ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _stripeSecretKey = _configuration["Stripe:SecretKey"];
            _stripePublishableKey = _configuration["Stripe:PublishableKey"];
            _userManager = userManager;
            //_ticketGenerator = ticketGenerator;
        }
        //// GET: Payment/Index


        public IActionResult Index(int bookingId, decimal totalPrice)
        {
            var booking = _context.Bookings
                                  .Include(b => b.Movie)  // Ensure Movie is included
                                  .Include(b => b.BookedSeats)
                                  .ThenInclude(bs => bs.RowGroup)  // Include RowGroup to get the price
                                  .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Pass the total price and booking details to the view
            ViewData["TotalPrice"] = totalPrice;
            ViewData["BookingId"] = bookingId;
            ViewData["StripePublishableKey"] = _stripePublishableKey;

            return View();
        }


        // POST: Payment/Charge
        [HttpPost]
        public async Task<IActionResult> Charge(int bookingId, decimal totalPrice, string stripeToken)
        {
            StripeConfiguration.ApiKey = _stripeSecretKey;

            try
            {
                // Log the values received to help debug if needed
                Console.WriteLine($"BookingId: {bookingId}, TotalPrice: {totalPrice}, StripeToken: {stripeToken}");

                // Create a charge request to Stripe
                var options = new ChargeCreateOptions
                {
                    Amount = (long)(totalPrice * 100),  // Convert totalPrice to paise (100 paise = 1 INR)
                    Currency = "inr",  // Set currency to INR for Indian Rupees
                    Description = $"Movie Ticket Booking for Booking ID {bookingId}",
                    Source = stripeToken,  // Stripe token from the frontend
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                // After successful payment, update the booking status in the database
                if (charge.Status == "succeeded")
                {
                    var booking = _context.Bookings
                                          .FirstOrDefault(b => b.BookingId == bookingId);

                    if (booking != null)
                    {
                        //  booking.PaymentStatus = "Paid";  // Update payment status
                        _context.SaveChanges();
                    }

                    // Redirect to the Success page
                    return RedirectToAction("Success");
                }
                else
                {
                    // Handle failure
                    return RedirectToAction("Failure");
                }
            }
            catch (StripeException ex)
            {
                // Handle error from Stripe API
                return RedirectToAction("Failure", new { errorMessage = ex.Message });
            }
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failure(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }


        // POST: Payment/ProceedToPayment
        //[HttpPost]
        //public async Task<IActionResult> ProceedToPayment(int bookingId, decimal totalPrice)
        //{
        //    // Fetch the booking by ID
        //    var booking = _context.Bookings.Include(b => b.Movie)
        //                                   .Include(b => b.BookedSeats)
        //                                   .FirstOrDefault(b => b.BookingId == bookingId);

        //    if (booking == null)
        //    {
        //        return NotFound();
        //    }

        //    // Update payment status and user-related information
        //    booking.PaymentStatus = "Paid";
        //    booking.BookedPrice = totalPrice;

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = _userManager.GetUserId(User);
        //        booking.CustomerId = userId;
        //    }

        //    // Save the updated booking
        //    _context.Update(booking);
        //    await _context.SaveChangesAsync();

        //    // Generate the ticket with QR code and PDF
        //    var pdfFile = GenerateTicketPDF(booking);

        //    // Return the file for download
        //    return File(pdfFile, "application/pdf", "MovieTicket_" + booking.BookingId + ".pdf");
        //}

        //    [HttpPost]
        //    public async Task<IActionResult> ProceedToPayment(int bookingId, decimal totalPrice)
        //    {
        //        // Fetch the booking by ID
        //        var booking = await _context.Bookings
        //                                     .Include(b => b.Movie)
        //                                     .Include(b => b.BookedSeats)
        //                                     .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        //        if (booking == null)
        //        {
        //            // Return a NotFound response if the booking doesn't exist
        //            return NotFound();
        //        }

        //        // Update the payment status and total price
        //        booking.PaymentStatus = "Paid"; // Set payment status to 'Paid'
        //        booking.BookedPrice = totalPrice; // Set the booked price to the given totalPrice

        //        // If the user is authenticated, link the booking to the user
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            var userId = _userManager.GetUserId(User);
        //            booking.CustomerId = userId; // Associate the booking with the authenticated user
        //        }

        //        // Save the updated booking to the database
        //        _context.Update(booking);
        //        await _context.SaveChangesAsync();

        //        // If the payment status is successfully updated to "Paid", proceed to generate the ticket
        //        if (booking.PaymentStatus == "Paid")
        //        {
        //            // Generate the ticket PDF file with the QR code
        //            var pdfFile = _ticketGenerator.GenerateTicketPDF(booking);

        //            // Return the generated PDF file as a downloadable file
        //            return File(pdfFile, "application/pdf", $"MovieTicket_{booking.BookingId}.pdf");
        //        }

        //        // If something goes wrong with the payment status, return an error
        //        return BadRequest("Payment was not successfully processed.");
        //    }


        //// Optionally, you can have a GetTicket action if needed.
        //[HttpGet]
        //    public async Task<IActionResult> DownloadTicket(int bookingId)
        //    {
        //        var booking = _context.Bookings.Include(b => b.Movie)
        //                                       .Include(b => b.BookedSeats)
        //                                       .FirstOrDefault(b => b.BookingId == bookingId);
        //        if (booking == null)
        //        {
        //            return NotFound();
        //        }

        //        var pdfFile = GenerateTicketPDF(booking);

        //        // Return the file for download
        //        return File(pdfFile, "application/pdf", "MovieTicket_" + booking.BookingId + ".pdf");
        //    }

        //    private byte[] GenerateTicketPDF(Booking booking)
        //    {
        //        // Create the QR Code data as a string
        //        string qrCodeData = $"Movie: {booking.Movie.Title}\n" +
        //                            $"ShowTime: {booking.Movie.ShowTime}\n" +
        //                            $"BookedSeats: {string.Join(", ", booking.BookedSeats.Select(s => $"{s.RowName} {s.ColumnName}"))}";

        //        // Generate the QR code image
        //        using (var qrGenerator = new QRCodeGenerator())
        //        {
        //            var qrCodeDataObj = qrGenerator.CreateQrCode(qrCodeData, QRCodeGenerator.ECCLevel.Q);
        //            var qrCode = new QRCode(qrCodeDataObj);
        //            using (var qrImage = qrCode.GetGraphic(20))
        //            {
        //                using (var ms = new MemoryStream())
        //                {
        //                    qrImage.Save(ms, ImageFormat.Png);
        //                    var qrCodeImage = ms.ToArray();

        //                    // Now generate the PDF ticket
        //                    using (var msPdf = new MemoryStream())
        //                    {
        //                        // Create the PDF document with A4 page size
        //                        var document = new Document(iTextSharp.text.PageSize.A4);
        //                        var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, msPdf);
        //                        document.Open();

        //                        // Add the movie details to the PDF
        //                        document.Add(new iTextSharp.text.Paragraph($"Movie: {booking.Movie.Title}"));
        //                        document.Add(new iTextSharp.text.Paragraph($"ShowTime: {booking.Movie.ShowTime.ToString("dd-MM-yyyy HH:mm")}",
        //                            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
        //                        document.Add(new iTextSharp.text.Paragraph($"Booking ID: {booking.BookingId}"));
        //                        document.Add(new iTextSharp.text.Paragraph($"Customer Name: {booking.CustomerName}"));
        //                        document.Add(new iTextSharp.text.Paragraph($"Price: {booking.BookedPrice:C}"));

        //                        // Add booked seats details to the PDF
        //                        var seatDetails = new StringBuilder();
        //                        foreach (var seat in booking.BookedSeats)
        //                        {
        //                            seatDetails.AppendLine($"{seat.RowName} {seat.ColumnName}");
        //                        }
        //                        document.Add(new iTextSharp.text.Paragraph($"Booked Seats: {seatDetails.ToString()}"));

        //                        // Add the generated QR code image to the PDF
        //                        var img = iTextSharp.text.Image.GetInstance(qrCodeImage);
        //                        img.ScaleToFit(150f, 150f);  // Scale the image to fit into the page
        //                        document.Add(img);

        //                        // Close the document
        //                        document.Close();

        //                        // Return the generated PDF as a byte array
        //                        return msPdf.ToArray();
        //                    }
        //                }
        //            }
        //        }
        //    }



        //[HttpPost]
        //public async Task<IActionResult> ProceedToPayment(int bookingId, decimal totalPrice)
        //{
        //    // Fetch the booking by ID
        //    var booking = await _context.Bookings
        //                                 .Include(b => b.Movie)
        //                                 .Include(b => b.BookedSeats)
        //                                 .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        //    if (booking == null)
        //    {
        //        // Return a NotFound response if the booking doesn't exist
        //        return NotFound();
        //    }

        //    // Update the payment status and total price
        //    booking.PaymentStatus = "Paid"; // Set payment status to 'Paid'
        //    booking.BookedPrice = totalPrice; // Set the booked price to the given totalPrice

        //    // If the user is authenticated, link the booking to the user
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = _userManager.GetUserId(User);
        //        booking.CustomerId = userId; // Associate the booking with the authenticated user
        //    }

        //    // Save the updated booking to the database
        //    _context.Update(booking);
        //    await _context.SaveChangesAsync();

        //    // If the payment status is successfully updated to "Paid", proceed to generate the ticket
        //    if (booking.PaymentStatus == "Paid")
        //    {
        //        // Generate the ticket PDF file with the QR code
        //        var pdfFile = GenerateTicketPDF(booking);

        //        // Return the generated PDF file as a downloadable file
        //        return File(pdfFile, "application/pdf", $"MovieTicket_{booking.BookingId}.pdf");
        //    }

        //    // If something goes wrong with the payment status, return an error
        //    return BadRequest("Payment was not successfully processed.");
        //}

        // Optionally, you can have a GetTicket action if needed.



        [HttpPost]
        public async Task<IActionResult> ProceedToPayment(int bookingId, decimal totalPrice)
        {
            // Fetch the booking by ID
            var booking = await _context.Bookings
                                        .Include(b => b.Movie)
                                        .Include(b => b.BookedSeats)
                                        .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Update the payment status and total price
            booking.PaymentStatus = "Paid"; // Set payment status to 'Paid'
            booking.BookedPrice = totalPrice; // Set the booked price to the given totalPrice

            // If the user is authenticated, link the booking to the user
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                booking.CustomerId = userId; // Associate the booking with the authenticated user
            }

            // Save the updated booking to the database
            _context.Update(booking);
            await _context.SaveChangesAsync();

            // If the payment status is successfully updated to "Paid", proceed to generate the ticket
            if (booking.PaymentStatus == "Paid")
            {
                // Generate the ticket PDF file with the QR code
                var pdfFile = GenerateTicketPDF(booking);

                // Return the generated PDF file as a downloadable file
                return File(pdfFile, "application/pdf", $"MovieTicket_{booking.BookingId}.pdf");
            }

            // If something goes wrong with the payment status, return an error
            return BadRequest("Payment was not successfully processed.");
        }


 

 

    public byte[] GenerateQRCode(string data)
    {
        using (var qrGenerator = new QRCoder.Core.QRCodeGenerator())
        {
            // Create QR code data from the input string (ECC Level Q for better error correction)
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCoder.Core.QRCodeGenerator.ECCLevel.Q);

            using (var qrCode = new QRCode(qrCodeData))  // Generate the QR code object
            using (var bitmap = qrCode.GetGraphic(20))  // Get the graphical representation of the QR code (size 20)
            using (var ms = new MemoryStream())
            {
                // Save the bitmap image to the memory stream in PNG format
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                // Return the byte array representing the PNG image
                return ms.ToArray();
            }
        }
    }




    [HttpGet]
        public async Task<IActionResult> DownloadTicket(int bookingId)
        {
            var booking = await _context.Bookings
                                         .Include(b => b.Movie)
                                         .Include(b => b.BookedSeats)
                                         .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            var pdfFile = GenerateTicketPDF(booking);

            // Return the file for download
            return File(pdfFile, "application/pdf", $"MovieTicket_{booking.BookingId}.pdf");
        }
        public byte[] GenerateTicketPDF(Booking booking)
        {
            using (var ms = new MemoryStream())
            {
                // Initialize the PDF writer and document
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Add Booking Details
                document.Add(new Paragraph($"Booking ID: {booking.BookingId}"));
                document.Add(new Paragraph($"Customer Name: {booking.CustomerName}"));
                document.Add(new Paragraph($"Movie: {booking.Movie?.Title}"));
                document.Add(new Paragraph($"Booking Time: {booking.BookingTime.ToString("dd-MM-yyyy hh:mm:ss tt")}"));
                document.Add(new Paragraph($"Total Price: {booking.BookedPrice.ToString("C")}"));

                // Add Seat Details (loop through booked seats)
                var table = new Table(3, true);
                table.AddHeaderCell("Row");
                table.AddHeaderCell("Column");
                table.AddHeaderCell("Price");

                foreach (var seat in booking.BookedSeats)
                {
                    table.AddCell(seat.RowName.ToString());
                    table.AddCell(seat.ColumnName.ToString());
                    table.AddCell(seat.RowGroup.Price.ToString("C"));
                }

                document.Add(table);

                byte[] qrCodeImage = GenerateQRCode($"Booking ID: {booking.BookingId}"); // You can use other data here
                ImageData qrImage = ImageDataFactory.Create(qrCodeImage);  // No 'using' block here

                // Add QR Code to the document
                Image qrCode = new Image(qrImage).SetWidth(100).SetHeight(100).SetFixedPosition(450, 650); // Adjust position/size
                document.Add(qrCode);


                // Close the document
                document.Close();

                // Return the PDF as a byte array
                return ms.ToArray();
            }
        }



    }
        }
