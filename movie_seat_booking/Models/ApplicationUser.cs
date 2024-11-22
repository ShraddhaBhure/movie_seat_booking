using Microsoft.AspNetCore.Identity;
using System;



namespace movie_seat_booking.Models
{
    public class ApplicationUser : IdentityUser
    {  
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
       
    }
}
