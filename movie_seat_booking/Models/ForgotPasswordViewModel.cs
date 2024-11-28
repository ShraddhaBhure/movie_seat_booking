using System.ComponentModel.DataAnnotations;

namespace movie_seat_booking.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
