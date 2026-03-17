using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;
using System.Net.Sockets;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Restrict access to result management to admins only (Uncomment when ready)
    public class ResultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Inject database context for data access and web environment
        public ResultController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Displays the list of active exam results along with associated student details
        public IActionResult Index()
        {
            // Count trashed records for UI display
            ViewBag.TrashedCount = _context.Results.Where(c => c.IsTrashed).Count();

            // Fetch results that are NOT trashed and INCLUDE the related Student entity
            var result = _context.Results
                .Include(s => s.Student)
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(result);
        }

        // GET: Prepares and loads the form to create a new student result
        public IActionResult Create()
        {
            // Fetch list of active students to populate a dropdown menu in the view
            ViewBag.Students = _context.Students.Where(s => !s.IsTrashed).ToList();

            // Extract values from the ClassType enum to populate another dropdown menu
            ViewBag.ClassType = Enum.GetValues(typeof(ClassType))
                          .Cast<ClassType>()
                          .Select(e => new { Id = (int)e, Name = e.ToString() })
                          .ToList();

            return View();
        }

        // POST: Saves the new result data to the database
        [HttpPost]
        public IActionResult Create(Result result)
        {
            // Add the newly created result to the context
            _context.Results.Add(result);

            // Save changes to the actual database
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Loads the specific result for editing, along with necessary dropdown lists
        public IActionResult Edit(int id)
        {
            // Populate active students list for the dropdown
            ViewBag.Students = _context.Students.Where(s => !s.IsTrashed).ToList();

            // Populate ClassType enum for the dropdown
            ViewBag.ClassType = Enum.GetValues(typeof(ClassType))
                          .Cast<ClassType>()
                          .Select(e => new { Id = (int)e, Name = e.ToString() })
                          .ToList();

            // Fetch the specific result by ID and include the related student data
            var result = _context.Results
                .Include(s => s.Student)
                .Where(c => c.ResultId == id)
                .FirstOrDefault();

            return View(result);
        }

        // POST: Updates the modified result in the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure to prevent CSRF
        public IActionResult Edit(Result result)
        {
            // Retrieve the existing result record
            var existingResult = _context.Results
                .FirstOrDefault(c => c.ResultId == result.ResultId);

            if (existingResult == null)
            {
                return NotFound();
            }

            // Update fields manually
            existingResult.StudentId = result.StudentId;
            existingResult.Marks = result.Marks;
            existingResult.ClassType = result.ClassType;
            existingResult.CourseFees = result.CourseFees;
            existingResult.LastDateOfPayment = result.LastDateOfPayment;

            _context.SaveChanges(); // Commit updates to database

            return RedirectToAction("Index");
        }

        // GET: Displays the list of soft-deleted (trashed) results
        public IActionResult ViewTrash()
        {
            // Count total trashed results
            ViewBag.TrashedCount = _context.Results.Where(c => c.IsTrashed).Count();

            // Fetch only trashed results
            var result = _context.Results
                .Where(c => c.IsTrashed)
                .ToList();

            return View(result);
        }

        // GET: Soft deletes a result by moving it to the trash
        public IActionResult Trash(int id)
        {
            var result = _context.Results
                .FirstOrDefault(c => c.ResultId == id);

            if (result == null)
            {
                return NotFound();
            }

            result.IsTrashed = true; // Mark as trashed
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }

        // GET: Restores a soft-deleted result back to the active list
        public IActionResult Restore(int id)
        {
            var result = _context.Results
                .FirstOrDefault(c => c.ResultId == id);

            if (result == null)
            {
                return NotFound();
            }

            result.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }
    }
}