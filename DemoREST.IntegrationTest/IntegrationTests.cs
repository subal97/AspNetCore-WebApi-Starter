using DemoREST.Contracts.V1;
using DemoREST.Contracts.V1.Requests;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace DemoREST.IntegrationTest
{
    public class IntegrationTests
    {
        protected readonly HttpClient _client;

        public IntegrationTests()
        {
            var appFactory = new WebApplicationFactory<DemoREST.Options.JwtSettings>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services => 
                    {

                        var dbContextDescriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                        services.Remove(dbContextDescriptor);
                        var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                        services.Remove(dbConnectionDescriptor);

                        // Create open SqliteConnection so EF won't automatically close it.
                        services.AddSingleton<DbConnection>(container =>
                        {
                            var connection = new SqliteConnection("DataSource=:memory:");
                            connection.Open();

                            return connection;
                        });

                        services.AddDbContext<DataContext>((container, options) =>
                        {
                            var connection = container.GetRequiredService<DbConnection>();
                            options.UseSqlite(connection);
                        });
                    });
                });
            
            _client = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJWTAsync());
        }

        private async Task<string?> GetJWTAsync()
        {
            var response = await _client.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@integration.com",
                Password = "Integration@123"
            });

            var content = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            return content.Token;
        }
    }
}
