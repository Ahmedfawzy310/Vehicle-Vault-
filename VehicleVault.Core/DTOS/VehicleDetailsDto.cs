namespace VehicleVault.Core.DTOS
{
    public class VehicleDetailsDto
    {
        public string Model { get; set; } = string.Empty;
        public DateTime Year { get; set; }
        public decimal? PricePerDay { get; set; }
        public decimal? PricePerKilometer { get; set; }
        public bool DriverSupport { get; set; } = true;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public List<string> Images { get; set; } = default!;
        public List<DecorationDto>? Decorations { get; set; }
    }
}
