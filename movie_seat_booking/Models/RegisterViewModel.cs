namespace movie_seat_booking.Models
{
    public class RegisterViewModel
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PhoneNumber { get; set; }
   
        public string Role { get; set; }

        public List<string> Roles { get; set; } = new List<string> { "Customer", "Admin" };

    }
}
