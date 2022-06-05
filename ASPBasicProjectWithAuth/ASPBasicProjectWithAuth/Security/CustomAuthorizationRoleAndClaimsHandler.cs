using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPBasicProjectWithAuth.Security
{
    public class CustomAuthorizationRoleAndClaimsHandler : AuthorizationHandler<CustomAuthorizationRoleAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public CustomAuthorizationRoleAndClaimsHandler(
                IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizationRoleAndClaimsRequirement requirement)
        {

            if (httpContextAccessor == null)
            {
                return Task.CompletedTask;
            }
            string loggedInAdminId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = httpContextAccessor.HttpContext.Request.Query["userId"];
            if ((httpContextAccessor.HttpContext.User.IsInRole("Admin") && httpContextAccessor.HttpContext.User.HasClaim(x => x.Type == "Edit Role" && x.Value == "true")))       
                
                // || httpContextAccessor.HttpContext.User.IsInRole("SuperAdmin"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
