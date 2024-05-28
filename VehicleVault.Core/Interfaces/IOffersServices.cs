namespace VehicleVault.Core.Interfaces
{
    public interface IOffersServices
    {
        Task<Offer> GeneratePromoCodeAsync(PromoCodeDto code);
        Task<decimal> GetDiscountAmountAsync(string code);
        Task<bool> ValidatePromoCodeAsync(string code);
    }
}
