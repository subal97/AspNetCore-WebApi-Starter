using Microsoft.AspNetCore.Mvc;

namespace DemoREST.Contracts.V1.Requests.Queries
{
    public class PostsQuery
    {
        public string UserId { get; set; } = string.Empty;
    }
}
