global using Microsoft.AspNetCore.Http;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using VehicleVault.Core.Settings;


namespace VehicleVault.Ef.Repositories
{
    public class MailServices:IMailServices
    {
        private readonly EmailSettings _mailSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public MailServices(IOptions<EmailSettings> mailSettings, UserManager<ApplicationUser> userManager)
        {
            _mailSettings = mailSettings.Value;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null, string confirmationCode = null)
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.SenderEmail),
                Subject = subject
            };

            email.To.Add(MailboxAddress.Parse(mailTo));

            var builder = new BodyBuilder();

            if (attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.SenderEmail, _mailSettings.SmtpPassword);
            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }



    }
}
