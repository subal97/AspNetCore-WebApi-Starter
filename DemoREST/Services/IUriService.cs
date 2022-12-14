using DemoREST.Contracts.V1.Requests.Queries;

namespace DemoREST.Services
{
    public interface IUriService
    {
        Uri GetPostUri(string postId);

        Uri GetAllPostUri(PaginationQuery pagination = null);
    }
}
