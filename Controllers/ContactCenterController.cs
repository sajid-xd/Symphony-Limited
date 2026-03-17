using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Keep this commented until you want to restrict this to Admins only
    public class ContactCenterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Injecting the database context and web environment
        public ContactCenterController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Fetches the list of active (non-trashed) contact centers
        public IActionResult Index()
        {
            // Count how many contact centers are currently in the trash
            ViewBag.TrashedCount = _context.Centres.Where(c => c.IsTrashed).Count();

            // Retrieve all contact centers that are NOT soft-deleted
            var contactCentre = _context.Centres
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(contactCentre);
        }

        // GET: Opens the form to add a new contact center
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saves the new contact center data into the database
        [HttpPost]
        public IActionResult Create(Centre contactCentre)
        {
            // Verify if the submitted data passes all validation rules
            if (ModelState.IsValid)
            {
                _context.Centres.Add(contactCentre); // Add to EF core tracking
                _context.SaveChanges(); // Commit changes to the actual database
                return RedirectToAction("Index"); // Send user back to the list
            }

            // If validation fails, return the form with the user's data and error messages
            return View(contactCentre);
        }

        // GET: Loads the edit form with the data of a specific contact center by ID
        public IActionResult Edit(int id)
        {
            // Find the specific center from the database using its ID
            var contactCentre = _context.Centres
                .Where(c => c.CentreId == id)
                .FirstOrDefault();

            return View(contactCentre);
        }

        // POST: Updates the modified contact center data in the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure against CSRF attacks
        public IActionResult Edit(Centre contactCentre)
        {
            // Check if the modified data is valid
            if (!ModelState.IsValid)
            {
                return View(contactCentre);
            }

            // Mark the entire entity state as modified so EF Core updates all fields
            _context.Entry(contactCentre).State = EntityState.Modified;

            _context.SaveChanges(); // Save the updated record to the database

            return RedirectToAction("Index");
        }

        // GET: Displays a list of all contact centers that have been moved to the trash
        public IActionResult ViewTrash()
        {
            // Count total items present in the trash
            ViewBag.TrashedCount = _context.Centres.Where(c => c.IsTrashed).Count();

            // Fetch only those records which are currently marked as trashed
            var contactCentre = _context.Centres
                .Where(c => c.IsTrashed)
                .ToList();

            return View(contactCentre);
        }

        // GET: Performs a "Soft Delete" by moving an active record to the trash
        public IActionResult Trash(int id)
        {
            // Find the record to be trashed
            var contactCentre = _context.Centres
                .FirstOrDefault(c => c.CentreId == id);

            if (contactCentre == null)
            {
                return NotFound();
            }

            contactCentre.IsTrashed = true; // Assign true to mark it as trashed
            _context.SaveChanges();   // Save changes to DB

            return RedirectToAction("Index");
        }

        // GET: Restores a soft-deleted record back to the active list
        public IActionResult Restore(int id)
        {
            // Find the trashed record
            var contactCentre = _context.Centres
                .FirstOrDefault(c => c.CentreId == id);

            if (contactCentre == null)
            {
                return NotFound();
            }

            contactCentre.IsTrashed = false; // Assign false to un-trash the record
            _context.SaveChanges();   // Save changes to DB

            return RedirectToAction("Index");
        }
    }
}