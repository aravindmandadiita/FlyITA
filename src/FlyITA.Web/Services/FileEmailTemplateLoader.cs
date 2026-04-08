using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlyITA.Web.Services;

public class FileEmailTemplateLoader : IEmailTemplateLoader
{
    private readonly IWebHostEnvironment _env;
    private readonly IPCentralDataAccess _dataAccess;
    private readonly IContextManager _context;
    private readonly ILogger<FileEmailTemplateLoader> _logger;

    public FileEmailTemplateLoader(
        IWebHostEnvironment env,
        IPCentralDataAccess dataAccess,
        IContextManager context,
        ILogger<FileEmailTemplateLoader> logger)
    {
        _env = env;
        _dataAccess = dataAccess;
        _context = context;
        _logger = logger;
    }

    public async Task<string?> LoadTemplateAsync(string templateName, CancellationToken ct = default)
    {
        // Try local file first
        var path = Path.Combine(_env.ContentRootPath, "Templates", templateName);
        if (File.Exists(path))
        {
            _logger.LogDebug("Loading email template from file: {Path}", path);
            return await File.ReadAllTextAsync(path, ct);
        }

        // Fall back to sidecar (for program-specific templates deployed per-environment)
        _logger.LogDebug("Template file not found at {Path}, falling back to sidecar for {TemplateName}", path, templateName);
        return await _dataAccess.GetEmailTemplateAsync(templateName, _context.ProgramID);
    }
}
