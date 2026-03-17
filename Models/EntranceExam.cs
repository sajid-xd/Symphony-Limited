using System.ComponentModel.DataAnnotations;
using SymphonyLimited.Models.AdminModel;

namespace SymphonyLimited.Models
{
    // Represents the entrance examination schedule and details in the database
    public class EntranceExam : BaseEntity
    {
        // Primary Key: Unique identifier for each entrance exam record
        public int EntranceExamId { get; set; }

        // Ensures an exam date is selected and formats it specifically as a Date type
        [Required(ErrorMessage = "Exam Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Exam Date")]
        public DateTime ExamDate { get; set; }

        // Mandatory field for exam fees with validation to ensure the amount is not negative
        [Required(ErrorMessage = "Exam Fees is required.")]
        [Range(0, float.MaxValue, ErrorMessage = "Exam Fees must be a positive value.")]
        [Display(Name = "Exam Fees")]
        public float ExamFees { get; set; }

        // Required text field for exam details with a maximum character limit of 500
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}