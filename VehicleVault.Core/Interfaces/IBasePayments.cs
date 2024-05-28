namespace VehicleVault.Core.Interfaces
{
    public interface IBasePayments : IBaseRepository<Payment>
    {
        Task ProcessPayment(int rentalId, byte methodId);
    }
}
