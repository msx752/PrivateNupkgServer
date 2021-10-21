using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using privatenupkgserver.Enums;
using privatenupkgserver.Extensions;
using privatenupkgserver.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace privatenupkgserver.Handlers
{
    public class NupkgAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IOptions<NugetServerCredentialOption> nugetServerCredentialOption;
        private readonly IOptions<NugetServerOption> nugetServerOption;

        public NupkgAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> authenticationSchemeOptions,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<NugetServerCredentialOption> nugetServerCredentialOption,
            IOptions<NugetServerOption> nugetServerOption
            ) : base(authenticationSchemeOptions, logger, encoder, clock)
        {
            this.nugetServerCredentialOption = nugetServerCredentialOption;
            this.nugetServerOption = nugetServerOption;
        }

        private async Task<AuthenticateResult> ApiKeyCheck()
        {
            if (!Request.Headers.ContainsKey("X-NuGet-ApiKey"))
                return await Task.FromResult(AuthenticateResult.Fail("Missing X-NuGet-ApiKey"));

            var userApiKey = Request.Headers["X-NuGet-ApiKey"];
            if (!nugetServerCredentialOption.Value.ApiKey.Equals(userApiKey))
                return await Task.FromResult(AuthenticateResult.Fail("Invalid ApiKey"));
            return await SucceesResponse();
        }

        private async Task<AuthenticateResult> AuthorizationCheck()
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Missing Authorization");

                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];
                if (!(nugetServerCredentialOption.Value.Username.Equals(username) &&
                    nugetServerCredentialOption.Value.Password.Equals(password)))
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization"));
                }
            }
            catch
            {
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization"));
            }

            return await SucceesResponse();
        }

        private async Task<AuthenticateResult> SucceesResponse()
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "PrivateNupkgServer"),
                new Claim(ClaimTypes.Name, "PrivateNupkgServer")
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();
            else if (!Request.Path.StartsWithSegments(
                new PathString($"{nugetServerOption.Value.GetApiMajorVersionUrl()}{nugetServerOption.Value.Resources[NugetServerResourceType.PackagePublish]}")) &&
                endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null)
                return await AuthorizationCheck();
            else
                return await ApiKeyCheck();
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"\", charset=\"UTF-8\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}