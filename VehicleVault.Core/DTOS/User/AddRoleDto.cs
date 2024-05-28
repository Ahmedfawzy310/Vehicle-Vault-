namespace VehicleVault.Core.DTOS.User
{
    public class AddRoleDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
