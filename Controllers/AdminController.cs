using Microsoft.AspNetCore.Mvc;
using SymphonyLimited.Filter;
using Microsoft.AspNetCore.Http; // Added this comment: Session requires Http namespace
using System.Linq;

namespace SymphonyLimited.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Injecting the database context so the admin controller can talk to the database
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loads the main admin dashboard and fetches statistics
        [AdminAuthFilter] // Custom filter to ensure only logged-in admins can access this page
        public IActionResult AdminDashboard()
        {
            // Fetching counts to display on the dashboard widgets
            ViewBag.CoursesCount = _context.Courses.Where(c => !c.IsTrashed).Count();
            ViewBag.QuestionsCount = _context.Courses.Where(c => !c.IsTrashed).Count();
            ViewBag.StudentsCount = _context.Courses.Where(c => !c.IsTrashed).Count();
            ViewBag.ContactCentreCount = _context.Courses.Where(c => !c.IsTrashed).Count();

            return View();
        }



        // POST: Logs out the admin and destroys their session
        [HttpPost]
        public IActionResult Logout()
        {
            // Clears all active session data (like the AdminUser string)
            HttpContext.Session.Clear();

            // Redirects the logged-out admin back to the public Home page
            return RedirectToAction("Index", "Home");
        }
    }
}