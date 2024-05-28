namespace VehicleVault.Core.Entities
{
    public class Setting
    {
        public int Id { get; set; }
        [MaxLength(1500)]
        public string Name { get; set; }
    }
}
