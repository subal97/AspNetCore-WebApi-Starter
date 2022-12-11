namespace DemoREST.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null) return String.Empty;
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}
