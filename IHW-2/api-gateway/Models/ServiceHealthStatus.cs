namespace ApiGateway.Models
{
    public class ServiceHealthStatus
    {
        public string Status { get; set; } = "unknown";
    }

    public class GatewayHealthStatus
    {
        public string Status { get; set; } = "up";
        public Dictionary<string, string> Services { get; set; } = new Dictionary<string, string>();
    }
}
