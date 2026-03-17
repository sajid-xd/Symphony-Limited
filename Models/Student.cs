using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // Represents a Student record in the system, inheriting common tracking fields from BaseEntity
    public class Student : BaseEntity
    {
        // Primary Key: Unique identifier for each student
        public int StudentId { get; set; }

        // Mandatory field for the student's unique roll number or ID
        [Required(ErrorMessage = "Roll Number is required.")]
        [Display(Name = "Roll Number")]
        public string RollNumber { get; set; }

        // Mandatory field for the full name of the student
        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Student Name")]
        public string Name { get; set; }

        // Mandatory email field with built-in validation for correct email format
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // Mandatory phone field with built-in validation for phone number format
        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        // Foreign Key: Links the student to a specific course in the Courses table
        [Required(ErrorMessage = "Course is required.")]
        [Display(Name = "Course")]
        public int? CourseId { get; set; }  // nullable, but required check will prompt if null

        // Navigation Property: Allows Entity Framework to fetch related Course details for this student
        [Display(Name = "Course Details")]
        public Course Course { get; set; }  // navigation property

        // Mandatory field to record how the student paid (e.g., Cash, Card, Online)
        [Required(ErrorMessage = "Payment Mode is required.")]
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }

        // Mandatory field to store additional payment info (e.g., Transaction ID, Receipt No)
        [Required(ErrorMessage = "Payment Details are required.")]
        [Display(Name = "Payment Details")]
        public string PaymentDetails { get; set; }
    }
}