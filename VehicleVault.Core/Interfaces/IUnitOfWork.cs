namespace VehicleVault.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMailServices MailServices { get; }
        IUserServices UserServices { get; }
        IOffersServices OffersServices { get; }
        IBaseRepository<Offer> Offers { get; }
        IBaseRepository<State> States { get; }
        IBaseRepository<VehicleType> VehicleTypes { get; }
        IBaseRepository<AdminRequest> AdminRequests { get; }
        IBaseRepository<Decoration> Decorations { get; }
        IBaseRepository<Image> Images {  get; }
        IBaseRepository<PaymentMethod> PaymentMethods { get; }
        IBaseVehicles BaseVehicles {  get; }
        IBaseRental BaseRentals { get; }
        IAdminRequestsServices AdminRequestsServices { get; }
        IBasePayments BasePayments {  get; }
        IBaseFeedBacks BaseFeedBacks {  get; }
        int Complete();
    }
}
