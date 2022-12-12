using DemoREST.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DemoREST.Controllers.V1
{
    [ApiKeyAuth]
    public class SecretController : ControllerBase
    {
        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("Now you have all the secrets");
        }
    }
}
