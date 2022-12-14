using AutoMapper;
using DemoREST.Cache;
using DemoREST.Contracts.V1;
using DemoREST.Contracts.V1.Requests;
using DemoREST.Contracts.V1.Requests.Queries;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Domain;
using DemoREST.Extensions;
using DemoREST.Helpers;
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
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery]PostsQuery? postsQuery, [FromQuery]PaginationQuery paginationQuery)
        {
            var filter = _mapper.Map<PostsFilter>(postsQuery);
            var pagination = _mapper.Map<Pagination>(paginationQuery);
            
            var posts = await _postService.GetPostsAsync(filter, pagination);
            var postResponse = _mapper.Map<List<PostResponse>>(posts);

            if(pagination is null || pagination.PageSize < 1 || pagination.PageNumber < 1)
            {
                return Ok(postResponse);
            }

            var paginatedResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, postResponse);

            return Ok(paginatedResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute]Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if(post == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PostResponse>(post));
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post {
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
            };
            post.PostId = Guid.NewGuid();
            
            await _postService.CreatePostAsync(post);

            List<Tag> tags = Array.Empty<Tag>().ToList();
            if (postRequest.Tags != null)
            {
                tags = postRequest.Tags.Select(x => new Tag { TagName = x.TagName }).ToList();
                await _postService.UpdateTagsForPostAsync(post.PostId, tags);
                post.Tags = await _postService.GetPostTagsForPostAsync(post.PostId);
            }

            var locationUri = _uriService.GetPostUri(post.PostId.ToString());
            var response = _mapper.Map<PostResponse>(post);

            return Created(locationUri, response);
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

            var tags = new List<Tag>();
            if (request.Tags != null)
            {
                var tagsToUpdate = request.Tags.Select(x => new Tag { TagName = x.TagName }).ToList();
                await _postService.UpdateTagsForPostAsync(post.PostId, tagsToUpdate);
                post.Tags = await _postService.GetPostTagsForPostAsync(post.PostId);
            }

            return Ok(_mapper.Map<PostResponse>(post));
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
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
