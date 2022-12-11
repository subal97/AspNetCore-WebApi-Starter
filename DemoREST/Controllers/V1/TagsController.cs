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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;

        public TagsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Tag.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            return Ok(tags.Select(tag => new TagResponse
            {
                TagName = tag.TagName,
            }));
        }

        [HttpPost(ApiRoutes.Tag.Get)]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByName(tagName);
            if (tag is null)
            {
                return NotFound();
            }
            return Ok(new TagResponse
            {
                TagName = tag.TagName,
            });
        }

        [HttpPost(ApiRoutes.Tag.Create)]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            var tag = new Tag {TagName = request.TagName };

            await _postService.CreateTagAsync(tag);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var location = baseUrl + "/" + ApiRoutes.Tag.Get.Replace("{tagName}", tag.TagName);

            var response = new TagResponse
            {
                TagName = tag.TagName,
            };

            return Created(location, response);
        }

        [HttpDelete(ApiRoutes.Tag.Delete)]
        [Authorize(Policy = "MustHaveOraganizationEmail")]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);
            return deleted == true ? NoContent() : NotFound();
        }
    }
}
