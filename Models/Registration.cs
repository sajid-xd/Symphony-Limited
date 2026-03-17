using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SymphonyLimited.Models
{
    // This model handles the registration of new customers/users and inherits tracking from BaseEntity
    public class Registration : BaseEntity
    {
        // Primary Key: Unique ID for each registered user
        public int RegistrationId { get; set; }

        // Mandatory field for the user's full name, displayed as "Customer Name" on forms
        [Required]
        [Display(Name = "Customer Name")]
        public string FullName { get; set; }

        // Mandatory field to store the user's physical address
        [Required]
        public string Address { get; set; }

        // Mandatory field validated to ensure a proper phone number format
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        // Mandatory field validated to ensure a proper email address format
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Mandatory unique username for account identification
        [Required]
        public string Username { get; set; }

        // Mandatory field that masks characters for security as a password type
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Used for validation only; [NotMapped] ensures it is NOT created as a column in the database.
        // [Compare] checks if this matches the Password field during registration.
        [NotMapped]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        // Default role assignment for new registrations (currently commented out)
        //public string Role { get; set; } = "User";
    }
}