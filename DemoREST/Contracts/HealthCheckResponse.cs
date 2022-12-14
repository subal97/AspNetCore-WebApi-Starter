namespace DemoREST.Contracts
{
    public class HealthCheckResponse
    {
        public string OverallStatus { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
