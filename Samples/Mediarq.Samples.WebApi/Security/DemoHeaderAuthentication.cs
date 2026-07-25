using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mediarq.Samples.WebApi.Security;

/// <summary>
/// A deliberately trivial authentication scheme for this sample only: a caller is "authenticated" by
/// sending an <c>X-User</c> header, and gets an optional <c>manager</c> role via <c>X-Role</c>. This
/// exists purely so <c>Mediarq.Authorization</c>'s <see cref="ClaimsPrincipal"/>/policy check has
/// something real to evaluate without wiring an actual identity provider. Do not use header-based
/// "authentication" like this outside a demo — anyone can set an HTTP header.
/// </summary>
public sealed class DemoHeaderAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>The authentication scheme name registered in <c>Program.cs</c>.</summary>
    public const string SchemeName = "Demo";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-User", out var user) || string.IsNullOrWhiteSpace(user))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.Name, user!) };
        if (Request.Headers.TryGetValue("X-Role", out var role) && !string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role!));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
