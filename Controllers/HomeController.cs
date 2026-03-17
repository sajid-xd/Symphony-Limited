using Microsoft.AspNetCore.Mvc;
using SymphonyLimited.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace SymphonyLimited.Controllers
{
    public class HomeController : Controller
    {
        // Logger for recording application errors or information
        private readonly ILogger<HomeController> _logger;
        // Database context to fetch data for the public website
        private readonly ApplicationDbContext _context;

        // Constructor: Injecting the logger and database context
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Loads the main landing/home page of the website
        public IActionResult Index()
        {
            return View();
        }

        // GET: Loads the "About Us" page detailing the company's information
        public IActionResult about()
        {
            return View();
        }

        // GET: Loads the "Contact Us" page
        public IActionResult contact()
        {
            return View();
        }

        // GET: Loads the detailed view for a specific course
        public IActionResult coursedetails()
        {
            return View();
        }

        // GET: Fetches all available courses from the database to display on the public Courses page
        public IActionResult Courses()
        {
            // Query the database for courses that are NOT trashed (active courses)
            // Select only the required fields into a DTO (Data Transfer Object) format
            var courses = _context.Courses
                .Where(c => !c.IsTrashed)
                .Select(c => new CoursesData
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    BasicFees = c.BasicFees,
                    AdvancedFees = c.AdvancedFees,
                    FilePath = c.FilePath
                })
                .ToList();

            // Wrap the list of courses in a MasterData model before sending it to the view
            var model = new MasterData
            {
                CoursesDatas = courses
            };

            return View(model);
        }

        // GET: Loads the upcoming events page
        public IActionResult events()
        {
            return View();
        }

        // GET: Loads the pricing/fees structure page
        public IActionResult pricing()
        {
            return View();
        }

        // GET: Loads a starter/blank template page
        public IActionResult starterpage()
        {
            return View();
        }

        // GET: Loads the trainers/instructors profile page
        public IActionResult trainers()
        {
            return View();
        }

        // GET: Loads the Privacy Policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Global error handler view to show friendly error messages
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Passes the current request ID to help with debugging
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // --- View Models / DTOs (Data Transfer Objects) ---
    // These records define the shape of the data being sent from the controller to the view

    // Master wrapper model for the view
    public record MasterData
    {
        public List<CoursesData> CoursesDatas { get; set; } = new();
    }

    // Specific structure representing course details to be displayed to the public
    public record CoursesData
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float BasicFees { get; set; }
        public float AdvancedFees { get; set; }
        public string? FilePath { get; set; } // Nullable because image might not exist
    }
}