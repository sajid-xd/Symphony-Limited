using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Restrict access to student management to admins only (Uncomment when ready)
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Inject database context for data operations and web environment
        public StudentsController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Displays a list of all active students and includes their enrolled course details
        public IActionResult Index()
        {
            // Count trashed student records for UI display purposes
            ViewBag.TrashedCount = _context.Students.Where(c => c.IsTrashed).Count();

            // Fetch students who are NOT trashed and INCLUDE the related Course entity data
            var students = _context.Students
                .Include(c => c.Course)
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(students);
        }

        // GET: Prepares and loads the form to register a new student
        public IActionResult Create()
        {
            // Fetch the list of active courses to populate a dropdown menu in the View
            ViewBag.Courses = _context.Courses.Where(c => !c.IsTrashed).ToList();

            return View();
        }

        // POST: Saves the new student data to the database
        [HttpPost]
        public IActionResult Create(Student students)
        {
            // Add the newly created student record to the EF context
            _context.Students.Add(students);

            // Commit changes to the actual database
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Loads the specific student record for editing, along with necessary dropdown lists
        public IActionResult Edit(int id)
        {
            // Populate the active courses list for the dropdown
            ViewBag.Courses = _context.Courses.Where(c => !c.IsTrashed).ToList();

            // Fetch the specific student by ID and include the related Course data
            var students = _context.Students
                .Include(c => c.Course)
                .Where(c => c.StudentId == id)
                .FirstOrDefault();

            return View(students);
        }

        // POST: Updates the modified student details in the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Security measure to prevent CSRF attacks
        public IActionResult Edit(Student students)
        {
            // Retrieve the existing student record from the database
            var existingStudent = _context.Students
                .FirstOrDefault(c => c.StudentId == students.StudentId);

            if (existingStudent == null)
            {
                return NotFound();
            }

            // Update specific fields with new values provided by the user
            existingStudent.RollNumber = students.RollNumber;
            existingStudent.Name = students.Name;
            existingStudent.Email = students.Email;
            existingStudent.Phone = students.Phone;
            existingStudent.CourseId = students.CourseId;
            existingStudent.PaymentMode = students.PaymentMode;
            existingStudent.PaymentDetails = students.PaymentDetails;

            _context.SaveChanges(); // Commit updates to the database

            return RedirectToAction("Index");
        }

        // GET: Displays the list of soft-deleted (trashed) students
        public IActionResult ViewTrash()
        {
            // Count total trashed student records
            ViewBag.TrashedCount = _context.Students.Where(c => c.IsTrashed).Count();

            // Fetch only trashed student records
            var students = _context.Students
                .Where(c => c.IsTrashed)
                .ToList();

            return View(students);
        }

        // GET: Soft deletes a student record by moving it to the trash
        public IActionResult Trash(int id)
        {
            var students = _context.Students
                .FirstOrDefault(c => c.StudentId == id);

            if (students == null)
            {
                return NotFound();
            }

            students.IsTrashed = true; // Mark as trashed instead of hard deleting
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }

        // GET: Restores a soft-deleted student record back to the active list
        public IActionResult Restore(int id)
        {
            var students = _context.Students
                .FirstOrDefault(c => c.StudentId == id);

            if (students == null)
            {
                return NotFound();
            }

            students.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();   // Commit to database

            return RedirectToAction("Index");
        }
    }
}