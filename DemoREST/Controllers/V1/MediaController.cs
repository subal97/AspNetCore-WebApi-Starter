using DemoREST.Contracts.V1;
using DemoREST.Contracts.V1.Requests;
using DemoREST.Contracts.V1.Responses;
using DemoREST.Domain;
using DemoREST.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoREST.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class MediaController : Controller
    {
        private readonly IPostService _postService;

        public MediaController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Media.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var media = await _postService.GetAllMediaAsync();
            return Ok(media.Select(x => new MediaResponse{
                MediaId = x.Id,
                PostId = x.PostId == String.Empty ? string.Empty : x.PostId.ToString(),
                Content = x.Content
            }).ToList());
        }

        [HttpPost(ApiRoutes.Media.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMediaRequest request)
        {
            var media = new Media
            {
                Content = request.Content
            };

            await _postService.CreateMediaAsync(new List<Media> {media});
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Media.Get.Replace("{mediaId}", media.Id.ToString());

            var response = new MediaResponse { MediaId = media.Id, Content = media.Content, PostId=null };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Media.Delete)]
        [Authorize(Policy = "MustHaveOraganizationEmail")]
        public async Task<IActionResult> Delete([FromRoute] int mediaId)
        {
            var deleted = await _postService.DeleteMediaAsync(mediaId);
            return deleted == true ? NoContent() : NotFound();
        }
    }
}
