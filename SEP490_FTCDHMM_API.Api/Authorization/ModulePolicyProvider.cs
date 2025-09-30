using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SEP490_FTCDHMM_API.Api.Authorization
{
    public class ModulePolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public ModulePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var parts = policyName.Split(':', 2);
            if (parts.Length == 2)
            {
                var domain = parts[0];
                var action = parts[1];

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ModulePermissionRequirement(domain, action))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }
    }
}
