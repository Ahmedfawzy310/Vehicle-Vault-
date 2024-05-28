namespace VehicleVault.Core.DTOS.Auth
{
    public class RegisterModel
    {
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(128)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
        [StringLength(50)]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public int StateId { get; set; }
    }
}
