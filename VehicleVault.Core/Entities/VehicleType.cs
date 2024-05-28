using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleVault.Core.Entities
{
    public class VehicleType
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
