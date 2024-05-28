using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleVault.Core.Entities
{
    public class PaymentMethod
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Payment> Payments { get; set; } = default!;
    }
}
