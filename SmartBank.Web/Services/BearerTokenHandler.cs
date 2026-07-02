using System.Net.Http.Headers;

namespace SmartBank.Web.Services
{
    /// <summary>
    /// After a user signs in, the JWT issued by AuthService is stashed as a claim on the
    /// SmartBank.Web auth cookie (see Pages/Account/Login.cshtml.cs). This handler reads that
    /// claim on every outgoing request and forwards it as a Bearer token to the API Gateway,
    /// so that any downstream microservice that later adds [Authorize] keeps working unchanged.
    /// </summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        public const string TokenClaimType = "smartbank_jwt";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var token = user?.FindFirst(TokenClaimType)?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
