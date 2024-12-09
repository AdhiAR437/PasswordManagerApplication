using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerApplication.Data;
using PasswordManagerApplication.Models;
using System.Linq;
using System.Threading.Tasks;



namespace PasswordManagerApplication.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }



        // GET: /Account/Login
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Login()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult AccountPage()
        {
            return View();
        }



        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Validate the input
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            // Retrieve the user based on the email
            var user = await _context.Users_tb.SingleOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                // User not found
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            // Verify the password
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                // Invalid password
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            // Successful login, set session variables
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetInt32("IsActive", 1); // Mark the user as active

            // Redirect to the home page or dashboard
            //return RedirectToAction("Home", "Home");
            return Ok("Login Successful");
        }



        // GET: /Account/Register
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]

        public IActionResult Register()
        {
            return View();
        }



        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User userModel)
        {
            // Validate input (ensure that the form data is not null or empty)
            if (userModel == null || string.IsNullOrEmpty(userModel.Email) || string.IsNullOrEmpty(userModel.PasswordHash))
            {
                return BadRequest("Email and password are required.");
            }

            // Check if the user already exists in the database
            var existingUser = await _context.Users_tb.FirstOrDefaultAsync(u => u.Email == userModel.Email);

            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "A user with this email already exists.";
                return RedirectToAction("UserSignup", "Signup");
            }

            // Ensure the model is valid before proceeding
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Hash the password before saving
            userModel.PasswordHash = _passwordHasher.HashPassword(userModel, userModel.PasswordHash);

            try
            {
                // Save the user to the database
                await _context.Users_tb.AddAsync(userModel);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while registering the user. Please try again later.";
                return StatusCode(500, new { message = "An error occurred while registering.Please try again later." });
            }

            // Successful registration, set session variables
            HttpContext.Session.SetString("UserEmail", userModel.Email);
            HttpContext.Session.SetInt32("IsActive", 1);

            return Ok("Registration Successful");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPassword([FromBody] PasswordVerfication request)
        {
            var username= HttpContext.Session.GetString("UserEmail");
            string password = request.Password;
            var user = await _context.Users_tb.SingleOrDefaultAsync(u => u.Email == username);
            if (user == null)
            {
                // User not found
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            // Verify the password
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                // Invalid password
                TempData["ErrorMessage"] = "Invalid email or password.";
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            return Ok("Verified Password");

        }



        // POST: /Account/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session data to log the user out
            HttpContext.Session.Clear();

            return Ok("Logout Successful");
            // Redirect to the login page or home page (depending on your app flow)
            //return RedirectToAction("Login", "Account"); // You can change this to "Home" if you prefer
        }

    }
}
