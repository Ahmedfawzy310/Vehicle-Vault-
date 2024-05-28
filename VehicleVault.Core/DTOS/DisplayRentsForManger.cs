namespace VehicleVault.Core.DTOS
{
    public class DisplayRentsForManger
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool DriverIncluded { get; set; }
        public decimal TotalCost { get; set; }
        public string RentedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Model { get; set; }
    }

}
