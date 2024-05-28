namespace VehicleVault.Core.DTOS
{
    public class DisplayRentsForUser
    {
        public string Model { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
    }
}
