namespace VehicleVault.Core.Entities
{
    public class RentalDecoration
    {
        public int RentalId { get; set; }
        public Rental Rental { get; set; }
        public int DecorationId { get; set; }
        public Decoration Decoration { get; set; }
    }
}
