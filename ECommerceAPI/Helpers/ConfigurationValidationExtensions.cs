namespace ECommerceAPI.Helpers
{
    public static class ConfigurationValidationExtensions
    {
        public static void ValidateJwtSettings(this IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            if (string.IsNullOrEmpty(jwtSettings["SecretKey"]))
                throw new InvalidOperationException("JWT SecretKey is missing in appsettings.json");
            if (string.IsNullOrEmpty(jwtSettings["Issuer"]))
                throw new InvalidOperationException("JWT Issuer is missing in appsettings.json");
            if (string.IsNullOrEmpty(jwtSettings["Audience"]))
                throw new InvalidOperationException("JWT Audience is missing in appsettings.json");
        }
    }
}
