using DemoREST.Data;
using DemoREST.Domain;
using Microsoft.EntityFrameworkCore;

namespace DemoREST.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<Post> GetPostByIdAsync(Guid postId)
        {
            return _dataContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);
        }

        public Task<List<Post>> GetPostsAsync()
        {
            return _dataContext.Posts.ToListAsync();
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            _dataContext.Posts.Add(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            if (!PostExists(postToUpdate.Id)) return false;
            _dataContext.Entry(postToUpdate).State = EntityState.Modified;
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }
        
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null) return false;
            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
        
        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleAsync(x => x.Id == postId);
            if(post == null)
            {
                return false;
            }

            return post.UserId == userId;
        }

        public Task<List<Media>> GetMediaForPostAsync(Guid postId)
        {
            return _dataContext.Media.Where(media => media.PostId == postId.ToString()).ToListAsync();
        }

        public async Task<bool> CreateMediaAsync(List<Media> media)
        {
            await _dataContext.Media.AddRangeAsync(media);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public Task<List<Media>> GetAllMediaAsync()
        {
            return _dataContext.Media.ToListAsync();
        }

        public async Task<bool> DeleteMediaAsync(int mediaId)
        {
            var media = await _dataContext.Media.SingleOrDefaultAsync(x => x.Id == mediaId);
            if(media == null)
            {
                return false;
            }
            _dataContext.Media.Remove(media);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        private bool PostExists(Guid id)
        {
            return _dataContext.Posts.Any(post => post.Id == id);
        }

    }
}
