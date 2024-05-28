namespace VehicleVault.Core.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Promocode { get; set; } = string.Empty;
        [DataType(DataType.Currency)]
        public decimal DiscountPercent { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
