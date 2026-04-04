using Azure.Core;

namespace Listjj.Infrastructure.Services.Credentials
{
    /// <summary>
    /// Abstraction for credential management.
    /// Allows different authentication strategies (service principal, managed identity, etc.)
    /// </summary>
    public interface ICredentialProvider
    {
        /// <summary>
        /// Get the token credential for authenticating with Azure services
        /// </summary>
        /// <returns>TokenCredential for Azure authentication</returns>
        TokenCredential GetCredential();
    }
}
