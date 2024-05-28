namespace VehicleVault.Core.DTOS
{
    public class UpdateDecorationDto
    {
        public int? Id { get; set; }  // Null if this is a new decoration
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
