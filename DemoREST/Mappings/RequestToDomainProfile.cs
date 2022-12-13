using AutoMapper;
using DemoREST.Contracts.V1.Requests.Queries;
using DemoREST.Domain;

namespace DemoREST.Mappings
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery, Pagination>();
        }
    }
}
