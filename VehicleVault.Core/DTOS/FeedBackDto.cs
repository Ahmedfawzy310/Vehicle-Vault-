namespace VehicleVault.Core.DTOS
{
    public class FeedBackDto
    {
        public string Comment { get; set; } = string.Empty;      
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public int VehicleId { get; set; }
    }
}
