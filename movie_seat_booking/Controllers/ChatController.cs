using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_seat_booking.Models;

namespace movie_seat_booking.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            
            return View();
        }
        public async Task<IActionResult> Chatbot()
        {

            return View();
        }


        [HttpPost]
        public IActionResult GetBotResponse(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return Json("Please type a message.");
            }

            // Search the database for a matching question
            var botResponse = _context.FAQs
                .Where(f => userMessage.ToLower().Contains(f.Question.ToLower()))
                .FirstOrDefault();

            if (botResponse != null)
            {
                return Json(botResponse.Answer);
            }

            return Json("Sorry, I didn't understand that. Please try asking something else.");
        }


        //    [HttpPost]
        //    public IActionResult GetBotResponse(string userMessage)
        //    {
        //        if (string.IsNullOrEmpty(userMessage))
        //        {
        //            return Json("Please type a message.");
        //        }

        //        // Basic logic for the chatbot
        //        string botResponse = GenerateBotResponse(userMessage);
        //        return Json(botResponse);
        //    }

        //    // Changed the method name to avoid conflict with the controller action
        //    private string GenerateBotResponse(string userMessage)
        //    {
        //        // Simple bot logic
        //        if (userMessage.ToLower().Contains("hello"))
        //        {
        //            return "Hi there! How can I help you today?";
        //        }
        //        else if (userMessage.ToLower().Contains("bye"))
        //        {
        //            return "Goodbye! Have a nice day!";
        //        }
        //        else
        //        {
        //            return "Sorry, I didn't understand that.";
        //        }
        //    }
        //}
    }
}
