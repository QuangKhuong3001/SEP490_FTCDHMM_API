using Hangfire.Dashboard;
using SEP490_FTCDHMM_API.Domain.Constants;

namespace SEP490_FTCDHMM_API.Infrastructure.Security
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var http = context.GetHttpContext();
            return http.User.Identity?.IsAuthenticated == true &&
                   http.User.IsInRole(RoleConstants.Admin);
        }
    }
}
