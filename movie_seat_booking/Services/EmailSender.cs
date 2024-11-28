using Microsoft.Extensions.Options;
using movie_seat_booking.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace movie_seat_booking.Services
{


    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail; private readonly EmailSettings _emailSettings;


        public EmailSender(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string fromEmail, IOptions<EmailSettings> emailSettings)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _fromEmail = fromEmail; _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Set to true for HTML emails
            };

            mailMessage.To.Add(email);

            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                smtpClient.EnableSsl = true; // Enable SSL for secure connection

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    // Log the exception if necessary
                    throw new InvalidOperationException("Error sending email", ex);
                }
            }
        }
    }

}
