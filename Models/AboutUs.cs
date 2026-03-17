using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // Represents the About Us content in the database and inherits common tracking fields from BaseEntity
    public class AboutUs : BaseEntity
    {
        // Primary Key: Unique identifier for the About Us record
        public int AboutUsId { get; set; }

        // Stores the main text/content to be displayed on the About Us page
        public string Description { get; set; }
    }
}