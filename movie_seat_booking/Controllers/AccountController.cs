using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using movie_seat_booking.Models;
using Microsoft.AspNetCore.Authorization;
using movie_seat_booking.Services;


namespace movie_seat_booking.Controllers
{
      public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        //private readonly IEmailSender _emailSender; // Assuming you have an email sender service

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(/*IEmailSender emailSender*/ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger; /*_emailSender = emailSender;*/
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ////if (ModelState.IsValid)
            ////{
                // Check if the role exists, otherwise create it
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    var role = new IdentityRole(model.Role);
                    await _roleManager.CreateAsync(role);
                }

                // Create a new user object
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Age = model.Age,
                    Sex = model.Sex
                };

                // Register the user with the provided password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to the selected role
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home"); // Redirect to the homepage after successful registration
                }

                // Add errors to the ModelState if registration fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
           // }

            // Return the registration form with validation errors if there are any
            return View(model);
        }




        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed.
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { token = token, email = user.Email },
                protocol: Request.Scheme);

        //    await _emailSender.SendEmailAsync(
              //  user.Email,
              //  "Reset your password",
             //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
       {
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Error");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ResetPasswordConfirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }









        // Register User (Customer)
        //public IActionResult Register() => View();

        //[HttpGet]
        //public IActionResult Register()
        //{
        //    var model = new RegisterViewModel(); // Create an empty or default model
        //    return View(model); // Pass the model to the view
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //        // Check if the user is trying to register as Admin and validate
        //        if (model.Role == "Admin")
        //        {
        //            // Optionally, check if the current user is an Admin (for security reasons)
        //            var currentUser = await _userManager.GetUserAsync(User);
        //            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Admin"))
        //            {
        //                ModelState.AddModelError("", "You are not authorized to register as an admin.");
        //                return View(model);
        //            }
        //        }

        //        var user = new ApplicationUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            FullName = model.FullName,
        //            Age = model.Age,
        //            Sex = model.Sex
        //        };

        //        var result = await _userManager.CreateAsync(user, model.Password);

        //        if (result.Succeeded)
        //        {
        //            // Assign the selected role to the user
        //            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
        //            if (!roleResult.Succeeded)
        //            {
        //                // Handle failure to assign role
        //                foreach (var error in roleResult.Errors)
        //                {
        //                    ModelState.AddModelError("", error.Description);
        //                }
        //            }

        //            return RedirectToAction("Index", "Home");  // Redirect to a page after successful registration
        //        }

        //        // Add errors to ModelState if user creation fails
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //   // }

        //    // Return the same view if ModelState is not valid
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //        var user = new ApplicationUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            FullName = model.FullName,
        //            Age = model.Age,
        //            Sex = model.Sex
        //        };

        //        var result = await _userManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

        //            if (roleResult.Succeeded)
        //            {
        //                // Successfully registered and assigned role
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                // Log role assignment errors
        //                foreach (var error in roleResult.Errors)
        //                {
        //                    _logger.LogError($"Role Assignment Error: {error.Description}");
        //                    ModelState.AddModelError("", error.Description);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // Log user creation errors
        //            foreach (var error in result.Errors)
        //            {
        //                _logger.LogError($"User Creation Error: {error.Description}");
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
        //  //  }

        //    // Return the same view with validation errors if any
        //    return View(model);
        //}

        // Admin User Login
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //if (ModelState.IsValid)
            //{
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            //}
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Admin Dashboard
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
