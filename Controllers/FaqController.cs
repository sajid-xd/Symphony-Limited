using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Keeps FAQ management restricted to Admins (Uncomment when ready)
    public class FaqController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Injecting database context to interact with the FAQs table
        public FaqController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Displays a list of all active (non-trashed) FAQs
        public IActionResult Index()
        {
            // Count how many FAQs are currently in the trash bin
            ViewBag.TrashedCount = _context.FAQs.Where(c => c.IsTrashed).Count();

            // Fetch only those FAQs that are NOT soft-deleted
            var faq = _context.FAQs
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(faq);
        }

        // GET: Loads the form to create a new FAQ entry
        public IActionResult Create()
        {
            return View();
        }

        // POST: Validates and saves the new FAQ into the database
        [HttpPost]
        public IActionResult Create(FAQ faq)
        {
            // Verify if the user input passes all validation rules defined in the Model
            if (ModelState.IsValid)
            {
                _context.FAQs.Add(faq); // Add the new record to EF Core tracking
                _context.SaveChanges(); // Commit the transaction to the database
                return RedirectToAction("Index"); // Send the user back to the FAQ list
            }

            // If validation fails, return the form with existing data and error messages
            return View(faq);
        }

        // GET: Fetches a specific FAQ by ID and loads it into the edit form
        public IActionResult Edit(int id)
        {
            // Find the specific FAQ record from the database using its ID
            var faq = _context.FAQs
                .Where(c => c.FAQId == id)
                .FirstOrDefault();

            return View(faq);
        }

        // POST: Updates the modified FAQ details in the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure against Cross-Site Request Forgery
        public IActionResult Edit(FAQ faq)
        {
            // Check if the modified data is valid
            if (!ModelState.IsValid)
            {
                return View(faq);
            }

            // Mark the entire entity state as modified so EF Core updates all its fields
            _context.Entry(faq).State = EntityState.Modified;

            _context.SaveChanges(); // Save the updated record to the database

            return RedirectToAction("Index");
        }

        // GET: Displays a list of all FAQs that have been soft-deleted
        public IActionResult ViewTrash()
        {
            // Count total items present in the trash
            ViewBag.TrashedCount = _context.FAQs.Where(c => c.IsTrashed).Count();

            // Fetch only those records which are currently marked as trashed
            var faq = _context.FAQs
                .Where(c => c.IsTrashed)
                .ToList();

            return View(faq);
        }

        // GET: Performs a "Soft Delete" by moving an active FAQ to the trash
        public IActionResult Trash(int id)
        {
            // Find the FAQ record to be trashed
            var faq = _context.FAQs
                .FirstOrDefault(c => c.FAQId == id);

            if (faq == null)
            {
                return NotFound();
            }

            faq.IsTrashed = true; // Mark as trashed instead of permanently deleting
            _context.SaveChanges();   // Save changes to DB

            return RedirectToAction("Index");
        }

        // GET: Restores a soft-deleted FAQ back to the active list
        public IActionResult Restore(int id)
        {
            // Find the trashed FAQ record
            var faq = _context.FAQs
                .FirstOrDefault(c => c.FAQId == id);

            if (faq == null)
            {
                return NotFound();
            }

            faq.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();   // Save changes to DB

            return RedirectToAction("Index");
        }
    }
}