global using System.ComponentModel.DataAnnotations;

namespace VehicleVault.Core.DTOS.Auth
{
    public class RessetPassword
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
