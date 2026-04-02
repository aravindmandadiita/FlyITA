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

        // Check for seamless login token
        var seamlessToken = Request.Query["seamless_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(seamlessToken))
        {
            return ProcessSeamlessLogin(seamlessToken);
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

            // Validate signature if certificate is configured
            if (!string.IsNullOrEmpty(Options.IdpCertificateFile) && File.Exists(Options.IdpCertificateFile))
            {
                var cert = X509CertificateLoader.LoadCertificateFromFile(Options.IdpCertificateFile);
                if (!ValidateSignature(doc, cert))
                {
                    Logger.LogWarning("SAML response signature validation failed");
                    return AuthenticateResult.Fail("Invalid SAML response signature.");
                }
            }
            else if (Options.DebugMode)
            {
                Logger.LogWarning("SAML signature validation skipped — no certificate configured (debug mode)");
            }

            // Extract NameID and attributes
            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsMgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            var nameIdNode = doc.SelectSingleNode("//saml:NameID", nsMgr);
            var nameId = nameIdNode?.InnerText ?? string.Empty;

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

    private AuthenticateResult ProcessSeamlessLogin(string token)
    {
        try
        {
            // Seamless login: token contains pre-authenticated user info
            var sessionId = Guid.NewGuid().ToString();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, token),
                new Claim("SAMLSessionID", sessionId),
                new Claim("SeamlessLogin", "true")
            };

            _contextManager.SAMLSessionID = sessionId;
            _contextManager.LoginType = "Seamless";

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing seamless login");
            return AuthenticateResult.Fail($"Seamless login failed: {ex.Message}");
        }
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
