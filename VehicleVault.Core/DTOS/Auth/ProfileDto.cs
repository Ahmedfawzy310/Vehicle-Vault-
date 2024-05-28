namespace VehicleVault.Core.DTOS.Auth
{
    public class ProfileDto
    {
        public string? FullName { get; set; } = string.Empty;
        public int? StateId { get; set; }
        public string? Phone { get; set; }
        public string? StateName { get; set; }
        public string? Email { get; set; }
    }
}
