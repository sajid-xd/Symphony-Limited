using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SymphonyLimited.Filter;
using SymphonyLimited.Models;
// Added missing using statement for Path and Directory
using System.IO;

namespace SymphonyLimited.Controllers
{
    //[AdminAuthFilter] // Ensure only Admins can manage courses (Uncomment when ready)
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webEnviroment;

        // Constructor: Injecting database context and web environment (for saving files)
        public CoursesController(ApplicationDbContext context, IWebHostEnvironment webEnviroment)
        {
            _context = context;
            _webEnviroment = webEnviroment;
        }

        // GET: Displays a list of all active courses
        public IActionResult Index()
        {
            // Count how many courses are currently soft-deleted (in trash)
            ViewBag.TrashedCount = _context.Courses.Where(c => c.IsTrashed).Count();

            // Fetch all courses that are NOT trashed
            var courses = _context.Courses
                .Where(c => !c.IsTrashed)
                .ToList();

            return View(courses);
        }

        // GET: Opens the form to add a new course
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saves a new course along with its uploaded image
        [HttpPost]
        public IActionResult Create(Course course)
        {
            // Verify that all required fields are correctly filled
            if (ModelState.IsValid)
            {
                string filecopypath = null;

                // Check if the user has uploaded an image file
                if (course.CourseFileImage != null && course.CourseFileImage.Length > 0)
                {
                    // Define the folder path where images will be stored on the server
                    string uploadsFolder = Path.Combine(
                        _webEnviroment.WebRootPath,
                        "Images",
                        "CourseImages"
                    );

                    // Ensure the directory exists; create it if it doesn't
                    Directory.CreateDirectory(uploadsFolder);

                    // Generate a unique filename using GUID to prevent overwriting existing files
                    string fileName = Guid.NewGuid() + Path.GetExtension(course.CourseFileImage.FileName);

                    // Combine folder path and unique file name
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the uploaded file to the server's physical storage
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        course.CourseFileImage.CopyTo(fileStream);
                    }

                    // Store the relative path to save it in the database
                    filecopypath = "/Images/CourseImages/" + fileName;
                }

                // Assign the image path to the course model
                course.FilePath = filecopypath;

                // Save the new course record into the database
                _context.Courses.Add(course);
                _context.SaveChanges();

                // Return to the course listing page
                return RedirectToAction("Index");
            }

            // If validation fails, reload the form with existing data
            return View(course);
        }

        // GET: Loads the specific course data into the edit form
        public IActionResult Edit(int id)
        {
            // Retrieve the course by its ID
            var course = _context.Courses
                .Where(c => c.CourseId == id)
                .FirstOrDefault();

            return View(course);
        }

        // POST: Updates the existing course details and optionally its image
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF Protection
        public IActionResult Edit(Course course)
        {
            // Ensure form data is valid
            if (!ModelState.IsValid)
            {
                return View(course);
            }

            // Find the existing course record from the database
            var existingCourse = _context.Courses
                .FirstOrDefault(c => c.CourseId == course.CourseId);

            // If the course doesn't exist, return a 404 error
            if (existingCourse == null)
            {
                return NotFound();
            }

            // Update normal text fields with new values
            existingCourse.CourseName = course.CourseName;
            existingCourse.Description = course.Description;
            existingCourse.StartTime = course.StartTime;
            existingCourse.EndTime = course.EndTime;
            existingCourse.BasicFees = course.BasicFees;
            existingCourse.AdvancedFees = course.AdvancedFees;

            // Check if a NEW image has been uploaded during the edit
            if (course.CourseFileImage != null && course.CourseFileImage.Length > 0)
            {
                // Define the physical upload path
                string uploadsFolder = Path.Combine(
                    _webEnviroment.WebRootPath,
                    "Images",
                    "CourseImages"
                );

                Directory.CreateDirectory(uploadsFolder);

                // Create a new unique file name
                string fileName = Guid.NewGuid() + Path.GetExtension(course.CourseFileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Save the new image file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    course.CourseFileImage.CopyTo(fileStream);
                }

                // Update the database path to point to the new image
                existingCourse.FilePath = "/Images/CourseImages/" + fileName;
            }

            // Save all updates to the database
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Displays all soft-deleted courses
        public IActionResult ViewTrash()
        {
            // Count total items present in the trash
            ViewBag.TrashedCount = _context.Courses.Where(c => c.IsTrashed).Count();

            // Fetch only those courses which are marked as trashed
            var courses = _context.Courses
                .Where(c => c.IsTrashed)
                .ToList();

            return View(courses);
        }

        // GET: Moves an active course to the trash (Soft Delete)
        public IActionResult Trash(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            course.IsTrashed = true; // Mark as trashed
            _context.SaveChanges();   // Commit to DB

            return RedirectToAction("Index");
        }

        // GET: Restores a trashed course back to the active list
        public IActionResult Restore(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            course.IsTrashed = false; // Mark as active (un-trash)
            _context.SaveChanges();   // Commit to DB

            return RedirectToAction("Index");
        }
    }
}