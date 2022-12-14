using AutoMapper;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Domain;

namespace DemoREST.Mappings
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Tag, TagResponse>();

            CreateMap<Post, PostResponse>()
                .ForMember(x => x.Tags, option => option.MapFrom(x => x.Tags.Select(x => new TagResponse { TagName = x.TagName })));
        }
    }
}
