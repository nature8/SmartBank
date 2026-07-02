namespace SmartBank.Web.Services
{
    /// <summary>Bound from the "ApiSettings" section in appsettings.json.</summary>
    public class ApiSettings
    {
        public string GatewayBaseUrl { get; set; } = "http://localhost:5202/";
    }
}
