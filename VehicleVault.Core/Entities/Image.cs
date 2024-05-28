namespace VehicleVault.Core.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
