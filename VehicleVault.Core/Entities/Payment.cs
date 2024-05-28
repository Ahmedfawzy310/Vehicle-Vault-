namespace VehicleVault.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public byte MethodId { get; set; }
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [DataType(DataType.Time)]
        public DateTime PaymentDate { get; set; }
        public Rental Rental { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = default!;
    }
}
