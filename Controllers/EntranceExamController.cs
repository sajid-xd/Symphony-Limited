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
        // GET: Loads the enrollment form where user selects course
        public IActionResult Enroll(int id)
        {
            var emailToRegister = HttpContext.Session.GetString("UserEmail") ?? HttpContext.Session.GetString("AdminUser");

            if (string.IsNullOrEmpty(emailToRegister))
            {
                return RedirectToAction("Logins", "Account");
            }

            var exam = _context.EntranceExams.FirstOrDefault(e => e.EntranceExamId == id);
            if (exam == null) return NotFound();

            ViewBag.Courses = _context.Courses.Where(c => !c.IsTrashed).ToList();
            
            var enrollment = new ExamEnrollment { EntranceExamId = exam.EntranceExamId, EntranceExam = exam, UserEmail = emailToRegister };
            return View(enrollment);
        }

        // POST: Enrolls the user, generates Hall Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(ExamEnrollment enrollment)
        {
            var emailToRegister = HttpContext.Session.GetString("UserEmail") ?? HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(emailToRegister)) return RedirectToAction("Logins", "Account");

            // Verify if already enrolled
            var existing = _context.ExamEnrollments.FirstOrDefault(e => e.EntranceExamId == enrollment.EntranceExamId && e.UserEmail == emailToRegister);
            if (existing != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this exam.";
                return RedirectToAction("Index");
            }

            enrollment.HallTicketNumber = "HT-" + DateTime.Now.ToString("yyMMdd") + new Random().Next(1000, 9999).ToString();
            enrollment.UserEmail = emailToRegister;
            enrollment.EnrollmentDate = DateTime.Now;
            enrollment.Status = "Pending";

            _context.ExamEnrollments.Add(enrollment);
            _context.SaveChanges();

            return RedirectToAction("EnrollSuccess", new { id = enrollment.EnrollmentId });
        }

        // GET: Success page displaying Hall Ticket
        public IActionResult EnrollSuccess(int id)
        {
            var enrollment = _context.ExamEnrollments
                .Include(e => e.EntranceExam)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.EnrollmentId == id);

            if (enrollment == null) return NotFound();
            
            return View(enrollment);
        }

        // GET: Check Result by Hall Ticket Number
        public IActionResult CheckResult(string ticketNumber)
        {
            if (string.IsNullOrEmpty(ticketNumber))
            {
                TempData["ErrorMessage"] = "Please enter a valid Hall Ticket Number.";
                return RedirectToAction("Index");
            }

            var enrollment = _context.ExamEnrollments
                .Include(e => e.EntranceExam)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.HallTicketNumber == ticketNumber);

            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "No records found for that Hall Ticket Number.";
                return RedirectToAction("Index");
            }

            if (enrollment.Status == "Passed")
            {
                var student = _context.Students.FirstOrDefault(s => s.Email == enrollment.UserEmail);
                if (student != null)
                {
                    ViewBag.Result = _context.Results.FirstOrDefault(r => r.StudentId == student.StudentId);
                }
            }

            // Fetch Applicant details
            var registration = _context.Registration.FirstOrDefault(r => r.Email == enrollment.UserEmail);
            if (registration != null)
            {
                ViewBag.FullName = registration.FullName;
                ViewBag.Phone = registration.PhoneNumber;
            }

            return View(enrollment);
        }

        // GET: Displays all enrollments for admins
        public IActionResult Enrollments()
        {
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser)) return RedirectToAction("Logins", "Account");

            // Fetch enrollments with corresponding exam details
            var enrollments = _context.ExamEnrollments
                .Include(e => e.EntranceExam)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToList();

            return View(enrollments);
        }

        // POST: Update Enrollment Status (Passed, Failed, Pending)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser)) return RedirectToAction("Logins", "Account");

            var enrollment = _context.ExamEnrollments.Find(id);
            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "Enrollment not found.";
                return RedirectToAction(nameof(Enrollments));
            }

            if (status == "Passed" || status == "Failed" || status == "Pending")
            {
                enrollment.Status = status;

                // Auto-provision Student and Result entries if Passed
                if (status == "Passed" && enrollment.CourseId.HasValue)
                {
                    // Check if student already exists for this email
                    var student = _context.Students.FirstOrDefault(s => s.Email == enrollment.UserEmail);
                    if (student == null)
                    {
                        var reg = _context.Registration.FirstOrDefault(r => r.Email == enrollment.UserEmail);
                        student = new SymphonyLimited.Models.Student
                        {
                            RollNumber = enrollment.HallTicketNumber ?? "ROLL-" + new Random().Next(1000, 9999),
                            Name = reg?.FullName ?? "New Student",
                            Email = enrollment.UserEmail,
                            Phone = reg?.PhoneNumber ?? "0000000000",
                            CourseId = enrollment.CourseId,
                            PaymentMode = "Pending",
                            PaymentDetails = "Pending"
                        };
                        _context.Students.Add(student);
                        _context.SaveChanges(); // Need ID for Result
                    }

                    // Check if Result already exists
                    var existingResult = _context.Results.FirstOrDefault(r => r.StudentId == student.StudentId);
                    if (existingResult == null)
                    {
                        var course = _context.Courses.Find(enrollment.CourseId);
                        
                        var newResult = new SymphonyLimited.Models.Result
                        {
                            StudentId = student.StudentId,
                            Marks = new Random().Next(65, 95), // Generate passing marks
                            ClassType = SymphonyLimited.Models.ClassType.Basic,
                            CourseFees = course?.BasicFees ?? 0,
                            LastDateOfPayment = DateTime.Now.AddDays(15)
                        };
                        _context.Results.Add(newResult);
                    }
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Enrollment #{id} status marked as {status}.";
            }

            return RedirectToAction(nameof(Enrollments));
        }

        // POST: Delete Enrollment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEnrollment(int id)
        {
            var adminUser = HttpContext.Session.GetString("AdminUser");
            if (string.IsNullOrEmpty(adminUser)) return RedirectToAction("Logins", "Account");

            var enrollment = _context.ExamEnrollments.Find(id);
            if (enrollment != null)
            {
                _context.ExamEnrollments.Remove(enrollment);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Enrollment record deleted successfully.";
            }

            return RedirectToAction(nameof(Enrollments));
        }
    }
}