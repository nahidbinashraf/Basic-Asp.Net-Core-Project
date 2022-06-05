using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPBasicProjectWithAuth.Security
{
    public class CustomAuthorizationSuperAdminHandler : AuthorizationHandler<CustomAuthorizationRoleAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CustomAuthorizationSuperAdminHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizationRoleAndClaimsRequirement requirement)
        {
            if (context.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        } 
    

    }
}
