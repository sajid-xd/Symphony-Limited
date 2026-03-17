using SymphonyLimited.Models.AdminModel;
using System.ComponentModel.DataAnnotations;

namespace SymphonyLimited.Models
{
    // Represents a physical training centre or branch in the database, inheriting tracking fields from BaseEntity
    public class Centre : BaseEntity
    {
        // Primary Key: Unique identifier for each contact centre
        public int CentreId { get; set; }

        // Stores the name of the training centre/branch
        public string CentreName { get; set; }

        // Stores the physical location/address of the centre
        public string Address { get; set; }

        // Stores the contact phone number for the centre
        public string PhoneNumber { get; set; }
    }
}