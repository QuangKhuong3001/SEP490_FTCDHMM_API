using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace SEP490_FTCDHMM_API.Api.Authorization
{
    public class ModulePermissionHandler : AuthorizationHandler<ModulePermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ModulePermissionRequirement requirement)
        {
            var permissionClaims = context.User.FindAll("Permissions");

            if (permissionClaims != null && permissionClaims.Any())
            {
                var permissions = permissionClaims.Select(c => c.Value).ToList();

                if (permissions.Contains(requirement.ToString()))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
