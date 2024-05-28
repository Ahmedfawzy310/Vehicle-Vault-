namespace VehicleVault.Core.DTOS.Auth
{
    public class TokenRequestModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
