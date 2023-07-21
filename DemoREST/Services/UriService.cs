using DemoREST.Contracts.V1;
using DemoREST.Contracts.V1.Requests.Queries;
using Microsoft.AspNetCore.WebUtilities;

namespace DemoREST.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetAllPostUri(PaginationQuery pagination = null!)
        {
            if (pagination is null)
            {
                return new Uri(String.Empty);
            }
            
            var uri = String.Concat(_baseUri, ApiRoutes.Posts.GetAll);
            uri = QueryHelpers.AddQueryString(uri, "pageNumber", pagination.PageNumber.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", pagination.PageSize.ToString());
            
            return new Uri(uri);
        }

        public Uri GetPostUri(string postId)
        {
            return new Uri(_baseUri + ApiRoutes.Posts.Get.Replace("{postId}", postId));
        }
    }
}
