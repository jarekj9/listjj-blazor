namespace ListjjFrontEnd.Data
{
    public class AppSettings
    {
        public string ApiEndpoint { get; set; }
        public OidcAuthenticationConfig OidcAuthentication { get; set; }
    }

    public class OidcAuthenticationConfig
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseType { get; set; }
    }
}
