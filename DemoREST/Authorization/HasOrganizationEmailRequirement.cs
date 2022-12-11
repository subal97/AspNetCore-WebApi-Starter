using Microsoft.AspNetCore.Authorization;

namespace DemoREST.Authorization
{
    public class HasOrganizationEmailRequirement : IAuthorizationRequirement
    {
        public string Domain { get;}
        public HasOrganizationEmailRequirement(string domainName)
        {
            Domain = domainName;
        }

    }
}
