using Microsoft.AspNetCore.Http;
using VehicleVault.Core.Settings.Attributes;
using VehicleVault.Core.Settings;

namespace VehicleVault.Core.DTOS
{
    public class RentalDto
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool DriverIncluded { get; set; }
        public int[]? Decorations { get; set; }
        public string? PromoCode { get; set; } = string.Empty;
        public string? Massege { get; set; } = string.Empty;
        [AllowedExtenstions(VehiclesSettings.AllowdExtension),
        AllowedImagesSize(VehiclesSettings.AllowdFileSizeByByte)]
        public IFormFile FrontCardImg { get; set; }
        [AllowedExtenstions(VehiclesSettings.AllowdExtension),
        AllowedImagesSize(VehiclesSettings.AllowdFileSizeByByte)]
        public IFormFile BackCardImg { get; set; }
    }
}
