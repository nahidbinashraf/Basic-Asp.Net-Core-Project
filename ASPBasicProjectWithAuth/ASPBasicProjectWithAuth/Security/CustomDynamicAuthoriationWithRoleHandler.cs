using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPBasicProjectWithAuth.Security
{
    public class CustomDynamicAuthoriationWithRoleHandler : AuthorizationHandler<CustomDynamicAuthoriationWithRoleRequiremnt>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CustomDynamicAuthoriationWithRoleHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomDynamicAuthoriationWithRoleRequiremnt requirement)
        {

            var controllerName = httpContextAccessor.HttpContext.Request.RouteValues["controller"];
            var actionName = httpContextAccessor.HttpContext.Request.RouteValues["action"];
            if ((httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin") || httpContextAccessor.HttpContext.User.HasClaim(x => x.Type.ToLower() == controllerName.ToString().ToLower() &&  x.Value == "true")))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
