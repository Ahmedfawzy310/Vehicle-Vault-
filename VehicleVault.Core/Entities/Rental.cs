namespace VehicleVault.Core.Entities
{
    public class Rental
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime StarDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public bool DriverIncluded { get; set; }
        public decimal TotalCost { get; set; }
        public int? OfferId { get; set; }
        public string FrontCard { get; set; } = string.Empty;
        public string BackCard { get; set; } = string.Empty;
        public bool IsPaid { get; set; } = false;
        public bool IsActive { get; set; }
        public Vehicle Vehicle { get; set; } 
        public ICollection<RentalDecoration> RentalDecorations { get; set; }
        public Payment Payment { get; set; }
        public Offer Offer { get; set; }
    }
}
