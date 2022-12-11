using DemoREST.Domain;

namespace DemoREST.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);

        Task<List<Media>> GetMediaForPostAsync(Guid postId);
        Task<bool> CreateMediaAsync(List<Media> medias);
        Task<List<Media>> GetAllMediaAsync();
        Task<bool> DeleteMediaAsync(int mediaId);
    }
}
