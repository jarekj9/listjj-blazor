using Azure.Identity;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Listjj.Infrastructure.Services.Credentials
{
    /// <summary>
    /// Service principal based credential provider.
    /// Authenticates using Azure App Registration credentials.
    /// </summary>
    public class AzureCredentialProvider : ICredentialProvider
    {
        private readonly string _tenantId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly ILogger<AzureCredentialProvider> _logger;

        public AzureCredentialProvider(
            IConfiguration configuration,
            ILogger<AzureCredentialProvider> logger)
        {
            _logger = logger;

            var authConfig = configuration.GetSection("AzureAppCredentials");
            _tenantId = authConfig["TenantId"] ?? throw new InvalidOperationException("AzureAppCredentials:TenantId not configured");
            _clientId = authConfig["ClientId"] ?? throw new InvalidOperationException("AzureAppCredentials:ClientId not configured");
            _clientSecret = authConfig["ClientSecret"] ?? throw new InvalidOperationException("AzureAppCredentials:ClientSecret not configured");

            _logger.LogInformation("AzureAppCredentials initialized for TenantId: {TenantId}", _tenantId);
        }

        public TokenCredential GetCredential()
        {
            return new ClientSecretCredential(_tenantId, _clientId, _clientSecret);
        }
    }
}
