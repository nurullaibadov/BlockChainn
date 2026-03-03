using Microsoft.AspNetCore.Authorization;

namespace Blockchain.API.Authorization
{
    public class SuperAdminRequirement : IAuthorizationRequirement { }

    public class SuperAdminRequirementHandler : AuthorizationHandler<SuperAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext ctx, SuperAdminRequirement requirement)
        {
            if (ctx.User.IsInRole("SuperAdmin"))
                ctx.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
