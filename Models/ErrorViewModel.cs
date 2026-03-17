namespace SymphonyLimited.Models
{
    // This model is used to display error information on the Error page
    public class ErrorViewModel
    {
        // Stores the unique ID of the web request that caused the error (nullable)
        public string? RequestId { get; set; }

        // Logic to determine if the RequestId should be shown on the screen
        // It returns true if RequestId is not null or empty
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}