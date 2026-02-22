namespace ManuBackend.DTOs
{
    public class WorkOrderDto
    {
        public string WordOrderId { get; set; }=string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }="PLANNED";
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
