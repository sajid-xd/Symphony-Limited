namespace SymphonyLimited.Models.AdminModel
{
    // This model represents the Admin Login table in the database
    public class AdminLogin
    {
        // Primary Key: Unique ID for each admin user
        public int AdminLoginId { get; set; }

        // Stores the username required for admin authentication
        public string UserName { get; set; }

        // Stores the password for the admin account
        public string Password { get; set; }
    }
}