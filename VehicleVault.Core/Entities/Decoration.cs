namespace VehicleVault.Core.Entities
{
    public class Decoration
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
