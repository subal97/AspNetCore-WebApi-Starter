using DemoREST.Options;
using DemoREST.Services;
using StackExchange.Redis;

namespace DemoREST.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = new RedisSettings();
            configuration.GetSection(nameof(RedisSettings)).Bind(redisSettings);
            services.AddSingleton(redisSettings);

            if (!redisSettings.Enabled)
            {
                return;
            }

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
            });

            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
