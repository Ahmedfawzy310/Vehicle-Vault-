namespace VehicleVault.Core.Entities
{
    public class AdminRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string FrontCardImg { get; set; } = string.Empty;
        public string BackCardImg { get; set; } = string.Empty;
    }
}
