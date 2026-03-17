using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;

namespace SymphonyLimited.Controllers
{
    // [AdminAuthFilter] // Uncomment this later to restrict access only to admins
    public class AboutUsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Injecting DbContext to interact with the database
        public AboutUsController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Fetches active (non-trashed) About Us records to display on the main page
        public IActionResult Index()
        {
            // Count total items present in the trash
            ViewBag.TrashedCount = _context.AboutUs.Where(c => c.IsTrashed).Count();

            // Fetch only those records which are NOT in trash
            var aboutUs = _context.AboutUs
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(aboutUs);
        }

        // GET: Returns the view to create a new About Us entry
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saves the new About Us entry into the database
        [HttpPost]
        public IActionResult Create(AboutUs aboutUs)
        {
            // Check if user input is valid according to Model validations
            if (ModelState.IsValid)
            {
                _context.AboutUs.Add(aboutUs);
                _context.SaveChanges(); // Commits changes to DB
                return RedirectToAction("Index"); // Redirect to list view
            }

            return View(aboutUs); // Return same view with errors if validation fails
        }

        // GET: Fetches a specific record by ID for editing
        public IActionResult Edit(int id)
        {
            var aboutUs = _context.AboutUs
                .Where(c => c.AboutUsId == id)
                .FirstOrDefault();

            return View(aboutUs);
        }

        // POST: Updates the existing record with new data provided by user
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents Cross-Site Request Forgery attacks
        public IActionResult Edit(AboutUs aboutUs)
        {
            if (!ModelState.IsValid)
            {
                return View(aboutUs);
            }

            // Find the existing record in database
            var existingaboutUs = _context.AboutUs
                .FirstOrDefault(c => c.AboutUsId == aboutUs.AboutUsId);

            if (existingaboutUs == null)
            {
                return NotFound();
            }

            // Update only the description field
            existingaboutUs.Description = aboutUs.Description;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Displays all records that have been soft-deleted (moved to trash)
        public IActionResult ViewTrash()
        {
            ViewBag.TrashedCount = _context.AboutUs.Where(c => c.IsTrashed).Count();

            var aboutUs = _context.AboutUs
                .Where(c => c.IsTrashed)
                .ToList();

            return View(aboutUs);
        }

        // GET: Moves an active record to the trash (Soft Delete)
        public IActionResult Trash(int id)
        {
            var aboutUs = _context.AboutUs
                .FirstOrDefault(c => c.AboutUsId == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            aboutUs.IsTrashed = true; // Mark as trashed instead of deleting permanently
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Restores a trashed record back to the active list
        public IActionResult Restore(int id)
        {
            var aboutUs = _context.AboutUs
                .FirstOrDefault(c => c.AboutUsId == id);

            if (aboutUs == null)
            {
                return NotFound();
            }

            aboutUs.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}