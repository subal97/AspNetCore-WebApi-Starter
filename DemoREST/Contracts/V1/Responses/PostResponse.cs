using DemoREST.Domain;

namespace DemoREST.Contracts.V1.Responses
{
    public class PostResponse
    {
        public Guid PostId { get; set; }
        
        public string Name { get; set; }
        
        public string UserId { get; set; }
        
        public IEnumerable <TagResponse> Tags { get; set; }
    }
}
