namespace VehicleVault.Core.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;
        [DataType(DataType.DateTime)]
        public DateTime Year { get; set; }
        [DataType(DataType.Currency)]
        public decimal? PricePerDay { get; set; }
        [DataType(DataType.Currency)]
        public decimal? PricePerKilometer { get; set; }
        public bool DriverSupport { get; set; } = true;
        [MaxLength(150)]
        public string Street { get; set; } = string.Empty;
        [MaxLength(150)]
        public string City { get; set; } = string.Empty;
        public int StateId { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public byte TypeId { get; set; }
        public VehicleType Type { get; set; } = default!;
        public State State { get; set; } = default!;
        public ICollection<Image> Images { get; set; } 
        public ICollection<Decoration> Decorations { get; set; }
        public ICollection<FeedBack> FeedBacks { get; set; }
    }
}
