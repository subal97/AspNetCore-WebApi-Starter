namespace DemoREST.Contracts.V1.Requests
{
    public class UpdatePostRequest
    {
        public string Name { get; set; }
        public IEnumerable<TagRequest> Tags { get; set; }
    }
}
