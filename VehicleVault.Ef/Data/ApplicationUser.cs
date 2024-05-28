global using Microsoft.AspNetCore.Identity;
global using VehicleVault.Core.Entities;
global using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace VehicleVault.Ef.Data
{
    public class ApplicationUser:IdentityUser
    {
        [Length(5, 250)]
        public string FullName { get; set; } = string.Empty;
        [Length(14, 14)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Input must contain only digits.")]
        public string? DrivingLicenseNumber { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? ResetPasswordCode { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
        public int StateId { get; set; }
        public State State { get; set; } = default!;
    }
}
