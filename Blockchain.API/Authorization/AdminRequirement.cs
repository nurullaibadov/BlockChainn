using Microsoft.AspNetCore.Authorization;

namespace Blockchain.API.Authorization
{
    public class AdminRequirement : IAuthorizationRequirement { }

    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext ctx, AdminRequirement requirement)
        {
            if (ctx.User.IsInRole("Admin") || ctx.User.IsInRole("SuperAdmin"))
                ctx.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
