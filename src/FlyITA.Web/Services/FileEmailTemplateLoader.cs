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
        // Reject path traversal attempts
        if (string.IsNullOrWhiteSpace(templateName) ||
            templateName.Contains("..") ||
            Path.IsPathRooted(templateName))
        {
            _logger.LogWarning("Rejected invalid template name: {TemplateName}", templateName);
            return null;
        }

        // Try local file first
        try
        {
            var templatesDir = Path.Combine(_env.ContentRootPath, "Templates");
            var path = Path.GetFullPath(Path.Combine(templatesDir, templateName));

            // Ensure resolved path is still under Templates directory
            if (!path.StartsWith(Path.GetFullPath(templatesDir), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Template path escaped Templates directory: {Path}", path);
                return null;
            }

            if (File.Exists(path))
            {
                _logger.LogDebug("Loading email template from file: {Path}", path);
                return await File.ReadAllTextAsync(path, ct);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Failed to read template file {TemplateName}, falling back to sidecar", templateName);
        }

        // Fall back to sidecar (for program-specific templates deployed per-environment)
        _logger.LogDebug("Template file not found locally, falling back to sidecar for {TemplateName}", templateName);
        return await _dataAccess.GetEmailTemplateAsync(templateName, _context.ProgramID);
    }
}
