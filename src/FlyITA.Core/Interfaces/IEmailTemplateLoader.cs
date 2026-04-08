namespace FlyITA.Core.Interfaces;

public interface IEmailTemplateLoader
{
    Task<string?> LoadTemplateAsync(string templateName, CancellationToken ct = default);
}
