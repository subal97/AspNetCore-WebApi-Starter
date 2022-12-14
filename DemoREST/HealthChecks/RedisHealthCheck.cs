using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace DemoREST.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexor;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexor)
        {
            _connectionMultiplexor = connectionMultiplexor;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var db = _connectionMultiplexor.GetDatabase();
                db.StringGet("health");
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception e)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(e.Message));
            }
        }
    }
}
