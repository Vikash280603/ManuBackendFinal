namespace ManuBackend.Models
{
    public class WorkOrder
    {
        // Primary key - UUID format (string, not int)  
        public string WordOrderId { get; set; }=string.Empty;

        // Foreign key to Product  
        public int ProductId { get; set; }

        // Quantity to produce  
        public int Quantity { get; set; }

        // Current status  
        // Values: "PLANNED", "IN_PROGRESS", "COMPLETED", "QUALITY_DONE" 
        public string Status { get; set; }="PLANNED";

        // When scheduled to start/complete  
        public DateTime? ScheduledDate { get; set; }

        // When order was created 
        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;

        // When production completed (null until done)  
        public DateTime? CompletedAt { get; set; }
    }
}
