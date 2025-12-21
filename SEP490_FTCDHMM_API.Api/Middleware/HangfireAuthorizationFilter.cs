using Hangfire.Dashboard;


namespace SEP490_FTCDHMM_API.Api.Middleware
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }

}
