using DemoREST.Domain;

namespace DemoREST.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
        public IEnumerable<CreateMediaRequest> Media { get; set; }
    }
}
