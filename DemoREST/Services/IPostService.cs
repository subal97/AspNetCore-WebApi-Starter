using DemoREST.Contracts.V1.Requests;
using DemoREST.Domain;

namespace DemoREST.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(Pagination pagination);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
        Task<bool> UpdateTagsForPostAsync(Guid postId, IEnumerable<Tag> tags);
        Task<List<Tag>> GetTagsForPostAsync(Guid postId);
        Task<List<PostTag>> GetPostTagsForPostAsync(Guid postId);
        Task<List<Tag>> GetAllTagsAsync();
        Task<bool> CreateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(string tagName);
        Task<Tag> GetTagByName(string tagName);
    }
}
