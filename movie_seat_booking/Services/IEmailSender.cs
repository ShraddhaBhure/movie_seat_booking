namespace movie_seat_booking.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

    }
}
