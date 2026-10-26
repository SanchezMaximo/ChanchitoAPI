namespace Infrastructure.ExternalServices
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        // Secret used to sign the tokens. It must NOT be committed: it lives in user-secrets / environment variables.
        public string Key { get; set; } = string.Empty;

        public int ExpirationMinutes { get; set; } = 60;
    }
}
