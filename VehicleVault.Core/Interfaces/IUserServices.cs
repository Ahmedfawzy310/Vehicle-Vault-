using VehicleVault.Core.DTOS.Auth;

namespace VehicleVault.Core.Interfaces
{
    public interface IUserServices
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<AuthModel> RegisteConfirm(string userId, string code);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
        Task<bool> SendResetPasswordCodeAsync(string email);
    }
}
