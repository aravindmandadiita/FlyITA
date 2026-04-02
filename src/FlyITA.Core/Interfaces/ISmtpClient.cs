using System.Net.Mail;

namespace FlyITA.Core.Interfaces;

public interface ISmtpClient
{
    Task SendAsync(MailMessage message, CancellationToken cancellationToken = default);
}
