using Microsoft.AspNetCore.Http;

namespace VehicleVault.Core.Interfaces
{
    public interface IMailServices
    {
        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null, string confirmationCode = null);

    }
}
