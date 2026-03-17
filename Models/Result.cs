using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // Enum to define fixed categories for Class Type, ensuring data consistency
    public enum ClassType
    {
        Basic,
        Advanced
    }

    // Represents the examination results and payment status for students in the database
    public class Result : BaseEntity
    {
        // Primary Key: Unique identifier for each result record
        public int ResultId { get; set; }

        // Foreign Key: Links this result to a specific student in the Students table
        [Display(Name = "Select Student")]
        [Required(ErrorMessage = "Student Selection is required.")]
        public int? StudentId { get; set; }

        // Navigation Property: Allows easy access to the related Student data
        public Student Student { get; set; }

        // Mandatory field to store the numerical marks obtained by the student
        [Display(Name = "Marks")]
        [Required(ErrorMessage = "Marks are required.")]
        public float Marks { get; set; }

        // Stores the type of class (Basic or Advanced) using the predefined Enum
        [Display(Name = "Class Type")]
        [Required(ErrorMessage = "Class Type is required.")]
        public ClassType ClassType { get; set; }

        // Mandatory field to store the total fees applicable for the course
        [Display(Name = "Course Fees")]
        [Required(ErrorMessage = "Course Fees are required.")]
        public float CourseFees { get; set; }

        // Required date field to track the deadline for course fee payment
        [Display(Name = "Last Date Of Payment")]
        [Required(ErrorMessage = "Last Date Of Payment is required.")]
        public DateTime LastDateOfPayment { get; set; }
    }
}