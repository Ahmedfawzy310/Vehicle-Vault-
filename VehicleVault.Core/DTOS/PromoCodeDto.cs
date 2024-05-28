namespace VehicleVault.Core.DTOS
{
    public class PromoCodeDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Isused { get; set; }
    }
}
