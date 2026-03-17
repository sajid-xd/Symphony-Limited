using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Ensure only Admins can manage Entrance Exams (Uncomment when ready)
    public class EntranceExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Injecting the database context to interact with the EntranceExams table
        public EntranceExamController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Displays a list of all active (non-trashed) entrance exams
        public IActionResult Index()
        {
            // Count the total number of trashed exams for dashboard/UI display
            ViewBag.TrashedCount = _context.EntranceExams.Where(c => c.IsTrashed).Count();

            // Fetch all exams that are currently active
            var exam = _context.EntranceExams
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(exam);
        }

        // GET: Loads the form to create a new entrance exam schedule
        public IActionResult Create()
        {
            return View();
        }

        // POST: Validates and saves the new entrance exam into the database
        [HttpPost]
        public IActionResult Create(EntranceExam exam)
        {
            // Check if all fields (like Date, Fees, Description) are correctly filled
            if (ModelState.IsValid)
            {
                _context.EntranceExams.Add(exam); // Add record to EF Core
                _context.SaveChanges(); // Commit to the database
                return RedirectToAction("Index"); // Send user back to the list
            }

            // Return the form with validation errors if the data is invalid
            return View(exam);
        }

        // GET: Fetches a specific entrance exam by ID and loads it into the edit form
        public IActionResult Edit(int id)
        {
            // Find the exam record based on the provided ID
            var exam = _context.EntranceExams
                .Where(c => c.EntranceExamId == id)
                .FirstOrDefault();

            return View(exam);
        }

        // POST: Updates the modified exam details in the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure against CSRF attacks
        public IActionResult Edit(EntranceExam exam)
        {
            // Verify if the updated data is valid
            if (!ModelState.IsValid)
            {
                return View(exam);
            }

            // Fetch the existing record from the database to update it safely
            var existingExam = _context.EntranceExams
                .FirstOrDefault(c => c.EntranceExamId == exam.EntranceExamId);

            if (existingExam == null)
            {
                return NotFound();
            }

            // Apply updates to the specific fields
            existingExam.ExamDate = exam.ExamDate;
            existingExam.ExamFees = exam.ExamFees;
            existingExam.Description = exam.Description;

            _context.SaveChanges(); // Save the updated record

            return RedirectToAction("Index");
        }

        // GET: Displays a list of all entrance exams that have been soft-deleted
        public IActionResult ViewTrash()
        {
            // Count total trashed items
            ViewBag.TrashedCount = _context.EntranceExams.Where(c => c.IsTrashed).Count();

            // Fetch only the records marked as trashed
            var exam = _context.EntranceExams
                .Where(c => c.IsTrashed)
                .ToList();

            return View(exam);
        }

        // GET: Performs a "Soft Delete" by moving an active exam record to the trash
        public IActionResult Trash(int id)
        {
            var exam = _context.EntranceExams
                .FirstOrDefault(c => c.EntranceExamId == id);

            if (exam == null)
            {
                return NotFound();
            }

            exam.IsTrashed = true; // Mark as trashed instead of permanently deleting
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }

        // GET: Restores a soft-deleted exam back to the active list
        public IActionResult Restore(int id)
        {
            var exam = _context.EntranceExams
                .FirstOrDefault(c => c.EntranceExamId == id);

            if (exam == null)
            {
                return NotFound();
            }

            exam.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }
    }
}