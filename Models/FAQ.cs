using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // Represents a Frequently Asked Question (FAQ) record in the database, inheriting from BaseEntity
    public class FAQ : BaseEntity
    {
        // Primary Key: Unique identifier for each FAQ entry
        public int FAQId { get; set; }

        // Stores the question text that users commonly ask
        public string Question { get; set; }

        // Stores the detailed answer/response to the corresponding question
        public string Answer { get; set; }
    }
}