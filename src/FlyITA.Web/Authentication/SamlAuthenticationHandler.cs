using System.IO.Compression;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Options;

namespace FlyITA.Web.Authentication;

public class SamlAuthenticationHandler : AuthenticationHandler<SamlOptions>, IAuthenticationSignOutHandler
{
    private readonly IContextManager _contextManager;

    public SamlAuthenticationHandler(
        IOptionsMonitor<SamlOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IContextManager contextManager)
        : base(options, logger, encoder)
    {
        _contextManager = contextManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Options.Enabled)
            return AuthenticateResult.NoResult();

        // Check for SAML response in form POST from IDP
        if (Request.Method == "POST" && Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            var samlResponse = form["SAMLResponse"].FirstOrDefault();
            if (!string.IsNullOrEmpty(samlResponse))
            {
                return await ProcessSamlResponseAsync(samlResponse);
            }
        }

        // Check for existing session authentication
        var samlSessionId = _contextManager.SAMLSessionID;
        if (!string.IsNullOrEmpty(samlSessionId))
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _contextManager.RealUserID.ToString()),
                new Claim("SAMLSessionID", samlSessionId)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }

        return AuthenticateResult.NoResult();
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (!Options.Enabled)
            return Task.CompletedTask;

        var authnRequest = BuildAuthnRequest(Options.SpEntityId, Options.AssertionConsumerServiceUrl);
        var encoded = DeflateAndEncode(authnRequest);
        var redirectUrl = $"{Options.IdpSsoUrl}?SAMLRequest={Uri.EscapeDataString(encoded)}";

        Response.Redirect(redirectUrl);
        return Task.CompletedTask;
    }

    public Task SignOutAsync(AuthenticationProperties? properties)
    {
        if (!Options.Enabled)
            return Task.CompletedTask;

        var logoutRequest = BuildLogoutRequest(Options.SpEntityId);
        var encoded = DeflateAndEncode(logoutRequest);
        var redirectUrl = $"{Options.IdpSloUrl}?SAMLRequest={Uri.EscapeDataString(encoded)}";

        _contextManager.SAMLSessionID = string.Empty;
        _contextManager.LoginType = string.Empty;

        Response.Redirect(redirectUrl);
        return Task.CompletedTask;
    }

    private async Task<AuthenticateResult> ProcessSamlResponseAsync(string base64Response)
    {
        try
        {
            var responseXml = Encoding.UTF8.GetString(Convert.FromBase64String(base64Response));
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(responseXml);

            // Validate signature — reject if no certificate unless debug mode
            if (string.IsNullOrEmpty(Options.IdpCertificateFile) || !File.Exists(Options.IdpCertificateFile))
            {
                if (Options.DebugMode)
                {
                    Logger.LogWarning("SAML signature validation skipped — no valid certificate configured (debug mode)");
                }
                else
                {
                    Logger.LogWarning("SAML response rejected — no IdP certificate configured");
                    return AuthenticateResult.Fail("SAML signature validation certificate is missing or invalid.");
                }
            }
            else
            {
                X509Certificate2 cert;
                try
                {
                    cert = X509CertificateLoader.LoadCertificateFromFile(Options.IdpCertificateFile);
                }
                catch (Exception ex)
                {
                    if (Options.DebugMode)
                    {
                        Logger.LogWarning(ex, "SAML signature validation skipped — certificate could not be loaded (debug mode)");
                        cert = null!;
                    }
                    else
                    {
                        Logger.LogError(ex, "SAML response rejected — IdP certificate could not be loaded");
                        return AuthenticateResult.Fail("SAML signature validation certificate could not be loaded.");
                    }
                }

                if (cert != null && !ValidateSignature(doc, cert))
                {
                    Logger.LogWarning("SAML response signature validation failed");
                    return AuthenticateResult.Fail("Invalid SAML response signature.");
                }
            }

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsMgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            // Validate SAML conditions (issuer, audience, time validity)
            var validationError = ValidateSamlConditions(doc, nsMgr);
            if (validationError != null)
            {
                Logger.LogWarning("SAML condition validation failed: {Error}", validationError);
                return AuthenticateResult.Fail(validationError);
            }

            // Extract NameID
            var nameIdNode = doc.SelectSingleNode("//saml:NameID", nsMgr);
            var nameId = nameIdNode?.InnerText;
            if (string.IsNullOrEmpty(nameId))
            {
                return AuthenticateResult.Fail("SAML response missing NameID.");
            }

            var sessionIndexNode = doc.SelectSingleNode("//saml:AuthnStatement/@SessionIndex", nsMgr);
            var sessionIndex = sessionIndexNode?.Value ?? Guid.NewGuid().ToString();

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nameId),
                new Claim("SAMLSessionID", sessionIndex)
            };

            // Extract attributes
            var attrNodes = doc.SelectNodes("//saml:Attribute", nsMgr);
            if (attrNodes != null)
            {
                foreach (XmlNode attr in attrNodes)
                {
                    var attrName = attr.Attributes?["Name"]?.Value;
                    var attrValue = attr.SelectSingleNode("saml:AttributeValue", nsMgr)?.InnerText;
                    if (attrName != null && attrValue != null)
                    {
                        claims.Add(new Claim(attrName, attrValue));
                    }
                }
            }

            // Set session properties
            _contextManager.SAMLSessionID = sessionIndex;
            _contextManager.LoginType = "SAML";

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing SAML response");
            return AuthenticateResult.Fail($"Error processing SAML response: {ex.Message}");
        }
    }

    private string? ValidateSamlConditions(XmlDocument doc, XmlNamespaceManager nsMgr)
    {
        // Validate issuer matches expected IDP
        var issuerNode = doc.SelectSingleNode("//saml:Issuer", nsMgr);
        // Issuer validation is informational for now — log but don't reject
        // (IDP entity ID may differ from SSO URL)
        if (issuerNode != null)
        {
            Logger.LogDebug("SAML response issuer: {Issuer}", issuerNode.InnerText);
        }

        // Validate audience restriction if present
        var audienceNode = doc.SelectSingleNode("//saml:Conditions/saml:AudienceRestriction/saml:Audience", nsMgr);
        if (audienceNode != null && !string.IsNullOrEmpty(Options.SpEntityId))
        {
            if (!string.Equals(audienceNode.InnerText, Options.SpEntityId, StringComparison.OrdinalIgnoreCase))
            {
                return $"SAML audience '{audienceNode.InnerText}' does not match SP entity ID '{Options.SpEntityId}'.";
            }
        }

        // Validate time conditions (NotBefore / NotOnOrAfter)
        var conditionsNode = doc.SelectSingleNode("//saml:Conditions", nsMgr);
        if (conditionsNode != null)
        {
            var clockSkew = TimeSpan.FromMinutes(5);
            var now = DateTime.UtcNow;

            var notBeforeAttr = conditionsNode.Attributes?["NotBefore"]?.Value;
            if (notBeforeAttr != null && DateTime.TryParse(notBeforeAttr, out var notBefore))
            {
                if (now < notBefore.ToUniversalTime() - clockSkew)
                    return $"SAML assertion is not yet valid (NotBefore: {notBefore:o}).";
            }

            var notOnOrAfterAttr = conditionsNode.Attributes?["NotOnOrAfter"]?.Value;
            if (notOnOrAfterAttr != null && DateTime.TryParse(notOnOrAfterAttr, out var notOnOrAfter))
            {
                if (now > notOnOrAfter.ToUniversalTime() + clockSkew)
                    return $"SAML assertion has expired (NotOnOrAfter: {notOnOrAfter:o}).";
            }
        }

        // Validate response status
        var statusCodeNode = doc.SelectSingleNode("//samlp:StatusCode/@Value", nsMgr);
        if (statusCodeNode != null && statusCodeNode.Value != "urn:oasis:names:tc:SAML:2.0:status:Success")
        {
            return $"SAML response status is not Success: {statusCodeNode.Value}";
        }

        return null;
    }

    private static bool ValidateSignature(XmlDocument doc, X509Certificate2 cert)
    {
        var signedXml = new SignedXml(doc);
        var signatureNode = doc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");

        if (signatureNode.Count == 0)
            return false;

        signedXml.LoadXml((XmlElement)signatureNode[0]!);
        return signedXml.CheckSignature(cert, true);
    }

    private static string BuildAuthnRequest(string spEntityId, string acsUrl)
    {
        var id = $"_id{Guid.NewGuid():N}";
        var issueInstant = DateTime.UtcNow.ToString("o");

        return $@"<samlp:AuthnRequest xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID=""{id}""
            Version=""2.0""
            IssueInstant=""{issueInstant}""
            AssertionConsumerServiceURL=""{acsUrl}""
            ProtocolBinding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"">
            <saml:Issuer xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"">{spEntityId}</saml:Issuer>
        </samlp:AuthnRequest>";
    }

    private static string BuildLogoutRequest(string spEntityId)
    {
        var id = $"_id{Guid.NewGuid():N}";
        var issueInstant = DateTime.UtcNow.ToString("o");

        return $@"<samlp:LogoutRequest xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol""
            ID=""{id}""
            Version=""2.0""
            IssueInstant=""{issueInstant}"">
            <saml:Issuer xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"">{spEntityId}</saml:Issuer>
        </samlp:LogoutRequest>";
    }

    private static string DeflateAndEncode(string xml)
    {
        var bytes = Encoding.UTF8.GetBytes(xml);
        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal))
        {
            deflate.Write(bytes, 0, bytes.Length);
        }
        return Convert.ToBase64String(output.ToArray());
    }
}
