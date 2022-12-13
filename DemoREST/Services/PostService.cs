using DemoREST.Contracts.V1.Requests;
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
            return _dataContext.Posts.AsNoTracking().Include(p=>p.Tags).SingleOrDefaultAsync(post => post.PostId == postId);
        }

        public Task<List<Post>> GetPostsAsync(Pagination pagination)
        {
            if(pagination is null)
            {
                return _dataContext.Posts.Include(p => p.Tags).ToListAsync();
            }

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            return _dataContext.Posts.Include(x => x.Tags)
                .Skip(skip)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            _dataContext.Posts.Add(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            if (!PostExists(postToUpdate.PostId)) return false;
            _dataContext.Entry(postToUpdate).State = EntityState.Modified;
            //_dataContext.Posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }
        
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post == null) return false;
            _dataContext.Posts.Remove(post);
            _dataContext.PostTag.RemoveRange(post.Tags);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
        
        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleAsync(x => x.PostId == postId);
            if(post == null)
            {
                return false;
            }

            return post.UserId == userId;
        }

        public Task<List<Tag>> GetTagsForPostAsync(Guid postId)
        {
            return _dataContext.PostTag.AsNoTracking().Where(p => p.PostId == postId).Select(p => new Tag { TagName = p.TagName}).ToListAsync();
        }

        public Task<List<PostTag>> GetPostTagsForPostAsync(Guid postId)
        {
            return _dataContext.PostTag.Where(x => x.PostId == postId).ToListAsync();
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            await _dataContext.Tag.AddRangeAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            return _dataContext.Tag.AsNoTracking().ToListAsync();
        }

        public Task<Tag> GetTagByName(string tagName)
        {
            return _dataContext.Tag.AsNoTracking().SingleOrDefaultAsync(tag => tag.TagName == tagName);
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _dataContext.Tag.SingleOrDefaultAsync(x => x.TagName == tagName);
            if (tag == null)
            {
                return false;
            }
            _dataContext.Tag.Remove(tag);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<bool> UpdateTagsForPostAsync(Guid postId, IEnumerable<Tag> tags)
        {
            var validTags = await _dataContext.Tag.AsNoTracking().ToListAsync();

            //keep the tags which are valid tags
            tags = tags.DistinctBy(x=>x.TagName);
            tags = (from tag in tags join validTag in validTags on tag.TagName equals validTag.TagName select tag).ToList();

            var existingTags = await _dataContext.PostTag.AsNoTracking().Where(p => p.PostId == postId).Select(x=> new Tag { TagName = x.TagName}).ToListAsync();
            
            var tagsToAdd = tags.ExceptBy(existingTags.Select(x => x.TagName), x => x.TagName)
                                .Select(x => new PostTag { TagName = x.TagName, PostId = postId }).ToList();
            
            _dataContext.PostTag.AddRange(tagsToAdd);
            await _dataContext.SaveChangesAsync();

            var tagsToRemove = existingTags.ExceptBy(tags.Select(x => x.TagName), x => x.TagName)
                                .Select(x => new PostTag { TagName = x.TagName, PostId = postId }).ToList();
            
            _dataContext.PostTag.RemoveRange(tagsToRemove);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        private bool PostExists(Guid id)
        {
            return _dataContext.Posts.Any(post => post.PostId == id);
        }

    }
}
