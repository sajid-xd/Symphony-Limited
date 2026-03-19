using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SymphonyLimited.Models
{
    // Represents an entrance exam enrollment by a user
    public class ExamEnrollment
    {
        // Primary Key for the enrollment record
        [Key]
        public int EnrollmentId { get; set; }

        // Foreign key to the Entrance Exam
        [Required]
        public int EntranceExamId { get; set; }

        // The user's email who enrolled in the exam
        [Required(ErrorMessage = "User Email is required.")]
        public string UserEmail { get; set; }

        // The timestamp when the user enrolled
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // The course they are applying for
        [Required(ErrorMessage = "Please select a course.")]
        public int? CourseId { get; set; }

        // The generated Hall Ticket Number
        public string? HallTicketNumber { get; set; }

        // The status of the enrollment (Pending, Passed, Failed)
        public string Status { get; set; } = "Pending";

        // Navigation properties
        [ForeignKey("EntranceExamId")]
        public EntranceExam? EntranceExam { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
