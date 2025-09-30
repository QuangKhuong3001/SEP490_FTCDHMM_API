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
            var permissionsClaim = context.User.FindFirst("Permissions")?.Value;

            if (!string.IsNullOrEmpty(permissionsClaim))
            {
                var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim);

                if (permissions != null &&
                    permissions.Contains(requirement.ToString()))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
