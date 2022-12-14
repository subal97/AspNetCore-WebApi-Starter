using DemoREST.Data;
using DemoREST.HealthChecks;

namespace DemoREST.Installers
{
    public class HealthCheckInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            if (environment.Equals("Production"))
            {
                services.AddHealthChecks().AddCheck<RedisHealthCheck>(name: "Redis");
            }
        }
    }
}
