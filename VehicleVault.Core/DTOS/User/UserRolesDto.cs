namespace VehicleVault.Core.DTOS.User
{
    public class UserRolesDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<RolesDto> Roles { get; set; } = default!;
    }
}
