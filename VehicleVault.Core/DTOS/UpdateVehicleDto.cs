using Microsoft.AspNetCore.Http;

namespace VehicleVault.Core.DTOS
{
    public class UpdateVehicleDto
    {
        public string? Model { get; set; }
        public decimal? PricePerDay { get; set; }
        public decimal? PricePerKilometer { get; set; }
        public bool? DriverSupport { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Description { get; set; }
        public int? StateId { get; set; }
        public byte? TypeId { get; set; }
        public DateTime? Year { get; set; }
        public DateTime? UpdatedDate {  get; set; }
        public List<IFormFile>? NewImages { get; set; }
        // IDs of images to delete
        public List<int>? DeleteImageIds { get; set; }

        // For decorations
        public List<int> DecorationIds { get; set; } = new List<int>();
        public List<string>? DecorationNames { get; set; } = new List<string>();
        public List<decimal>? DecorationPrices { get; set; } = new List<decimal>();
        public List<string>? DecorationDescriptions { get; set; } = new List<string>();
        public List<bool> DecorationDeleteFlags { get; set; } = new List<bool>();

        public List<UpdateDecorationDto> GetDecorations()
        {
            var decorations = new List<UpdateDecorationDto>();

            // Ensure there are elements to process
            int maxLength = new int[] {
        DecorationIds?.Count ?? 0,
        DecorationNames?.Count ?? 0,
        DecorationPrices?.Count ?? 0,
        DecorationDescriptions?.Count ?? 0,
        DecorationDeleteFlags?.Count ?? 0
    }.Max();

            for (int i = 0; i < maxLength; i++)
            {
                // Check for deletion scenario where only ID and flag might be present
                if (i < DecorationDeleteFlags?.Count && DecorationDeleteFlags[i] && i < DecorationIds?.Count)
                {
                    decorations.Add(new UpdateDecorationDto
                    {
                        Id = DecorationIds[i],
                        IsDeleted = DecorationDeleteFlags[i]
                    });
                }
                else if (i < DecorationNames?.Count && i < DecorationPrices?.Count && i < DecorationDescriptions?.Count)
                {
                    // Normal update/add scenario
                    decorations.Add(new UpdateDecorationDto
                    {
                        Id = DecorationIds?.Count > i ? DecorationIds[i] : null,
                        Name = DecorationNames[i],
                        Price = DecorationPrices[i],
                        Description = DecorationDescriptions[i],
                        IsDeleted = DecorationDeleteFlags?.Count > i ? DecorationDeleteFlags[i] : false
                    });
                }
            }

            return decorations;
        }


    }
}

