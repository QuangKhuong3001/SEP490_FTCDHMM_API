using Microsoft.AspNetCore.Authorization;

namespace SEP490_FTCDHMM_API.Api.Authorization
{
    public class ModulePermissionRequirement : IAuthorizationRequirement
    {
        public string Domain { get; }
        public string Action { get; }

        public ModulePermissionRequirement(string domain, string action)
        {
            Domain = domain;
            Action = action;
        }

        public override string ToString() => $"{Domain}:{Action}";
    }
}
