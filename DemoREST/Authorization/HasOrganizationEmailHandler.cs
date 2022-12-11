using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DemoREST.Authorization
{
    public class HasOrganizationEmailHandler : AuthorizationHandler<HasOrganizationEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasOrganizationEmailRequirement requirement)
        {
            string userEmail = context.User?.Claims.Single(x => x.Type == ClaimTypes.Email).Value ?? string.Empty;
            if (userEmail.EndsWith(requirement.Domain))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
