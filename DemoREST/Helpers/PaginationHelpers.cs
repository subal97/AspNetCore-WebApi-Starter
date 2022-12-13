using DemoREST.Contracts.V1.Requests.Queries;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Domain;
using DemoREST.Services;

namespace DemoREST.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, Pagination pagination, List<T> response)
        {
            var nextPage = pagination.PageNumber >= 1 ?
                uriService.GetAllPostUri(new PaginationQuery(pagination.PageNumber + 1, pagination.PageSize)).ToString() : null;

            var previousPage = pagination.PageNumber - 1 > 1 ?
                uriService.GetAllPostUri(new PaginationQuery(pagination.PageNumber - 1, pagination.PageSize)).ToString() : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : (int?)null,
                PageSize = pagination.PageSize >= 1 ? pagination.PageSize : (int?)null,
                NextPage = response.Any() ? nextPage : null,
                PreviousPage = pagination.PageNumber > 1 ? previousPage : null,
            };
        }
    }
}
