using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;
using Stripe;

namespace movie_seat_booking.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _stripeSecretKey;
        private readonly string _stripePublishableKey;
        public PaymentController(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _stripeSecretKey = _configuration["Stripe:SecretKey"];
            _stripePublishableKey = _configuration["Stripe:PublishableKey"];
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


        //// POST: Payment/Charge
        //[HttpPost]
        //public async Task<IActionResult> Charge(int bookingId, decimal totalPrice, string stripeToken)
        //{
        //    StripeConfiguration.ApiKey = _stripeSecretKey;

        //    try
        //    {
        //        // Create a charge request to Stripe
        //       var options = new ChargeCreateOptions
        //        {
        //            Amount = (long)(totalPrice * 100),  // Convert to paise (100 paise = 1 INR)
        //            Currency = "inr",  // Set currency to INR for Indian Rupees
        //            Description = $"Movie Ticket Booking for Booking ID {bookingId}",
        //            Source = stripeToken,  // Stripe token from the frontend
        //        };

        //        var service = new ChargeService();
        //        Charge charge = await service.CreateAsync(options);

        //        // After successful payment, update the booking status in the database
        //        if (charge.Status == "succeeded")
        //        {
        //            var booking = _context.Bookings
        //                                  .FirstOrDefault(b => b.BookingId == bookingId);

        //            if (booking != null)
        //            {
        //              //  booking.PaymentStatus = "Paid";  // Update payment status
        //                _context.SaveChanges();
        //            }

        //            // Redirect to the Success page
        //            return RedirectToAction("Success");
        //        }
        //        else
        //        {
        //            // Handle failure
        //            return RedirectToAction("Failure");
        //        }
        //    }
        //    catch (StripeException ex)
        //    {
        //        // Handle error from Stripe API
        //        return RedirectToAction("Failure", new { errorMessage = ex.Message });
        //    }
        //}
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
    }
}
