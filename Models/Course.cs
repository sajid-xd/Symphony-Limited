using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SymphonyLimited.Models
{
    // Represents a training course offered by Symphony Limited, inheriting common tracking fields
    public class Course : BaseEntity
    {
        // Primary Key: Unique identifier for the course
        public int CourseId { get; set; }

        // Enforces that Course Name is mandatory and limits its length to 100 characters
        [Required(ErrorMessage = "Please enter course name")]
        [StringLength(100, ErrorMessage = "Course name cannot exceed 100 characters")]
        public string CourseName { get; set; }

        // Enforces that the Description field cannot be left empty by the user
        [Required(ErrorMessage = "Please enter course description")]
        public string Description { get; set; }

        // Required field to specify when the course starts
        [Required(ErrorMessage = "Please select start time")]
        public DateTime StartTime { get; set; }

        // Required field to specify when the course ends
        [Required(ErrorMessage = "Please select end time")]
        public DateTime EndTime { get; set; }

        // Stores the fee amount for the basic tier of the course
        [Required(ErrorMessage = "Please enter basic fees")]
        public float BasicFees { get; set; }

        // Stores the fee amount for the advanced tier of the course
        [Required(ErrorMessage = "Please enter advanced fees")]
        public float AdvancedFees { get; set; }

        // Stores the relative path of the uploaded course image in the database (nullable)
        public string? FilePath { get; set; }

        // Used only to capture the uploaded image file from the HTML form.
        // [NotMapped] tells Entity Framework NOT to create a column for this in the database table.
        [NotMapped]
        public IFormFile CourseFileImage { get; set; }
    }
}