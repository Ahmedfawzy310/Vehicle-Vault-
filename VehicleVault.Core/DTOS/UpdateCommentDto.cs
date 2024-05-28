namespace VehicleVault.Core.DTOS
{
    public class UpdateCommentDto
    {
        public string Comment { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
