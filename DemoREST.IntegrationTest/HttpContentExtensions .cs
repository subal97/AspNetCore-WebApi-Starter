
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoREST.IntegrationTest
{
    public static class HttpContentExtensions
    {
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static async Task<T> ReadAsAsync<T>(this HttpContent content, JsonSerializerOptions options = null)
        {
            using(Stream stream = await content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options ?? defaultOptions);
            }
        }
    }
}
