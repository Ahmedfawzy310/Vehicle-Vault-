namespace VehicleVault.Core.DTOS.User
{
    public class UserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = default!;
    }
}
