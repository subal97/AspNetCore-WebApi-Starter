using DemoREST.Domain;

namespace DemoREST.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable <TagResponse> Tags { get; set; }
    }
}
