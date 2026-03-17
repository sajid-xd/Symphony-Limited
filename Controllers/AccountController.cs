using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Models;
using System.Linq;

namespace SymphonyLimited.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Injecting the database context to interact with the database
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LOGIN PAGE (GET): Displays the login form to the user
        public IActionResult Logins()
        {
            return View();
        }

        // LOGIN POST: Handles the login form submission and authenticates the user
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents Cross-Site Request Forgery (CSRF) attacks
        public IActionResult Logins(Login model)
        {
            // Verifies if the submitted form data meets the model validation rules
            if (ModelState.IsValid)
            {
                // Queries the database to find a matching user with the provided email and password
                var user = _context.Registration
                    .FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                // If user credentials are correct, establish a session
                if (user != null)
                {
                    // Storing user details in session variables for global access
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserName", user.FullName);
                    //HttpContext.Session.SetString("Role", user.Role);

                    //if (user.Role == "Admin")
                    //    return RedirectToAction("Index", "Admin");
                    //else

                    // Redirects authenticated user to the Home page
                    return RedirectToAction("Index", "Home");
                }

                // If credentials don't match, display an error message on the form
                ModelState.AddModelError("", "Invalid Email or Password");
            }

            // Returns the view with validation errors if the form is invalid
            return View(model);
        }

        // REGISTRATION PAGE (GET): Displays the customer registration form
        public IActionResult Registration()
        {
            return View();
        }

        // REGISTRATION POST: Handles new user registration form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registration(Registration model)
        {
            // Checks if all required fields are correctly filled by the user
            if (ModelState.IsValid)
            {
                //model.Role = "User";

                // Adds the new user record to the database context
                _context.Registration.Add(model);
                _context.SaveChanges(); // Commits the transaction to save data permanently

                // Redirects the user to the login page after successful registration
                return RedirectToAction("Logins");
            }

            // If validation fails, reloads the form showing the errors
            return View(model);
        }

        // LOGOUT: Terminates the user session securely
        public IActionResult Logout()
        {
            // Clears all session data associated with the current user
            HttpContext.Session.Clear();

            // Redirects the logged-out user back to the login screen
            return RedirectToAction("Logins");
        }
    }
}