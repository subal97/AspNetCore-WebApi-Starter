using DemoREST.Contracts.V1;
using DemoREST.Contracts.V1.Requests;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Domain;
using DemoREST.Extensions;
using DemoREST.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoREST.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();

            var mediaCollection = await _postService.GetAllMediaAsync();

            var response = from post in posts
                           select new PostResponse
                           {
                               Id = post.Id,
                               Name = post.Name,
                               Media = mediaCollection.Where(x => x.PostId == post.Id.ToString()).ToList(),
                           };

            return Ok(response);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if(post == null)
            {
                return NotFound();
            }
            return Ok(new PostResponse { Id = post.Id, Name = post.Name, Media = await _postService.GetMediaForPostAsync(post.Id)});
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post {
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
            };
            post.Id = Guid.NewGuid();
            
            await _postService.CreatePostAsync(post);

            List<Media> mediaCollection = Array.Empty<Media>().ToList();
            if (postRequest.Media != null)
            {
                mediaCollection = (from media in postRequest.Media
                                       select new Media
                                       {
                                           PostId = post.Id.ToString(),
                                           Content = media.Content
                                       }).ToList();
                await _postService.CreateMediaAsync(mediaCollection);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id, Name = post.Name, Media = mediaCollection};

            return Created(location, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    Error = "You do not own this post"
                });
            }
            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = request.Name;
            
            var updated = await _postService.UpdatePostAsync(post);
            if (!updated)
            {
                return NotFound();
            }
            var mediaCollection = Array.Empty<Media>().ToList();
            mediaCollection = await _postService.GetMediaForPostAsync(post.Id);
            return updated == true ? Ok(new PostResponse { Id = post.Id, Name = post.Name, Media = mediaCollection }) : NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
            {
                return BadRequest(new
                {
                    Error = "You do not own this post"
                });
            }
            var deleted = await _postService.DeletePostAsync(postId);
            return deleted == true ? Ok("Delete Success") : NotFound();
        }
    }
}
