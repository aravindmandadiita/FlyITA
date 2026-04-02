using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Options;

namespace FlyITA.Web.Services;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpOptions _options;

    public SmtpClientWrapper(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(MailMessage message, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();

        if (_options.DeliveryMethod == "PickupDirectory")
        {
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = _options.PickupDirectoryLocation;
        }
        else
        {
            client.Host = _options.Host;
            client.Port = _options.Port;
            client.EnableSsl = _options.EnableSsl;
            if (!string.IsNullOrEmpty(_options.UserName))
                client.Credentials = new NetworkCredential(_options.UserName, _options.Password);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
