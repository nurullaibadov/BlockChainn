using Hangfire.Dashboard;

namespace Blockchain.API.Hangfire
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            // Only allow authenticated admins in production
            return httpContext.User.Identity?.IsAuthenticated == true &&
                   httpContext.User.IsInRole("Admin");
        }
    }
}
