namespace VehicleVault.Core.DTOS.Auth
{
    public class ConfirmResetPassword
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
