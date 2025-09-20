using Microsoft.AspNetCore.Mvc.Filters;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Api.Filters
{
    public class DisallowAuthenticatedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                throw new AppException(AppResponseCode.ACCESS_DENIED);
            }
        }
    }
}