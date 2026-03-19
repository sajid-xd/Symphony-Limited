using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // This model is used to capture user credentials during the login process
    public class Login
    {
        // Primary Key: Unique ID for the login attempt/session record
        public int Id { get; set; }

        // Mandatory field that accepts either Username or Email
        [Required(ErrorMessage = "Email or Username is required.")]
        [Display(Name = "Email or Username")]
        public string EmailOrUsername { get; set; }

        // Mandatory field that treats the input as a secure password (hides characters)
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Optional checkbox to persist the user session in the browser
        public bool RememberMe { get; set; }
    }
}