using iTextSharp.text;                    // iTextSharp for PDF generation
using iTextSharp.text.pdf;                // iTextSharp for PDF writer and related operations
using QRCoder;                           // QR code generation
using System.Drawing;                    // For handling QR code image generation
using System.Drawing.Imaging;            // For image format (PNG)
using System.IO;                         // For MemoryStream
using System.Linq;                       // For LINQ (e.g., Select)
using movie_seat_booking.Models;
using System.Text;


namespace movie_seat_booking.Services
{
    //public class TicketGenerator
    //{
    //    // Method to generate the PDF with QR code and movie booking details
    //    public byte[] GenerateTicketPDF(Booking booking)
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
    //                        var document = new Document(PageSize.A4);
    //                        var writer = PdfWriter.GetInstance(document, msPdf);
    //                        document.Open();

    //                        // Add the movie details to the PDF
    //                        document.Add(new Paragraph($"Movie: {booking.Movie.Title}"));
    //                        document.Add(new Paragraph($"ShowTime: {booking.Movie.ShowTime.ToString("dd-MM-yyyy HH:mm")}",
    //                            new iTextSharp.text.Font(Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
    //                        document.Add(new Paragraph($"Booking ID: {booking.BookingId}"));
    //                        document.Add(new Paragraph($"Customer Name: {booking.CustomerName}"));
    //                        document.Add(new Paragraph($"Price: {booking.BookedPrice:C}"));

    //                        // Add booked seats details to the PDF
    //                        var seatDetails = new StringBuilder();
    //                        foreach (var seat in booking.BookedSeats)
    //                        {
    //                            seatDetails.AppendLine($"{seat.RowName} {seat.ColumnName}");
    //                        }
    //                        document.Add(new Paragraph($"Booked Seats: {seatDetails.ToString()}"));

    //                        // Add the generated QR code image to the PDF
    //                        var img = Image.GetInstance(qrCodeImage);
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
   // }
}