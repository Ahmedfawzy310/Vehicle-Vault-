global using VehicleVault.Core.DTOS;
global using VehicleVault.Ef.Data;

namespace VehicleVault.Ef.Repositories
{
    public class OffersServices:IOffersServices
    {
        private readonly ApplicationDbContext _context;

        public OffersServices(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Offer> GeneratePromoCodeAsync(PromoCodeDto dto)
        {
            var promoCode = await GenerateUniquePromoCodeAsync();
            Offer code = new()
            {
                Promocode = promoCode,
                DiscountPercent = dto.DiscountAmount,
                ExpireDate = dto.ExpirationDate,
                IsUsed=false
            };
            await _context.AddAsync(code);
            await _context.SaveChangesAsync();
            return code;
        }

        public async Task<decimal> GetDiscountAmountAsync(string code)
        {
            var promoCode = await _context.Offers.FirstOrDefaultAsync(p => p.Promocode == code && p.ExpireDate >= DateTime.Now && !p.IsUsed);
            return promoCode?.DiscountPercent ?? 0;
        }

        public async Task<bool> ValidatePromoCodeAsync(string code)
        {
            var promoCode = await _context.Offers.FirstOrDefaultAsync(p => p.Promocode == code && p.ExpireDate >= DateTime.Now && !p.IsUsed);
            return promoCode != null;
        }


        private async Task<string> GenerateUniquePromoCodeAsync()
        {
            var promoCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            while (await _context.Offers.AnyAsync(p => p.Promocode == promoCode))
            {
                promoCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            }

            return promoCode;
        }
    }
}
