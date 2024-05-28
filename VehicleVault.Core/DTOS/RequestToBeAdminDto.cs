using Microsoft.AspNetCore.Http;


namespace VehicleVault.Core.DTOS
{
    public class RequestToBeAdminDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; }
        public string GarageName { get; set; } = string.Empty;
        public IFormFile FrontCard { get; set; }
        public IFormFile BackCard { get; set; }
        public string? Messege { get; set; }
    }
}
