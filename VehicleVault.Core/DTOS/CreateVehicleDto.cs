using Microsoft.AspNetCore.Http;
using VehicleVault.Core.Settings.Attributes;
using VehicleVault.Core.Settings;

namespace VehicleVault.Core.DTOS
{
    public class CreateVehicleDto
    {
        public string Model { get; set; } = string.Empty;
        public DateTime Year { get; set; }
        public decimal? PricePerDay { get; set; }
        public decimal? PricePerKilometer { get; set; }
        public bool DriverSupport { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int StateId { get; set; }
        public byte TypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> DecorationNames { get; set; } = new List<string>();
        public List<decimal> DecorationPrices { get; set; } = new List<decimal>();
        public List<string> DecorationDescriptions { get; set; } = new List<string>();

        public List<DecorationDto> GetDecorations()
        {
            var decorations = new List<DecorationDto>();
            for (int i = 0; i < DecorationNames.Count; i++)
            {
                decorations.Add(new DecorationDto
                {
                    Name = DecorationNames[i],
                    Price = DecorationPrices[i],
                    Description = DecorationDescriptions[i]
                });
            }
            return decorations;
        }

        [AllowedExtenstions(VehiclesSettings.AllowdExtension),
         AllowedImagesSize(VehiclesSettings.AllowdFileSizeByByte)]
        public List<IFormFile> Images { get; set; }
    }
}
