namespace SymphonyLimited.Models.AdminModel
{
    // This is a base class meant to be inherited by other models to provide common tracking fields
    public class BaseEntity
    {
        // Flag used for 'Soft Delete' feature. If true, the record is in the trash (not permanently deleted)
        public bool IsTrashed { get; set; } = false;

        // Automatically records the exact date and time when a record is first created in the database
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Automatically records the date and time whenever a record is modified/updated
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}