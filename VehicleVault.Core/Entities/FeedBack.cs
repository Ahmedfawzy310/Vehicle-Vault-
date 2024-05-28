namespace VehicleVault.Core.Entities
{
    public class FeedBack
    {
        public int Id { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;
    }
}
