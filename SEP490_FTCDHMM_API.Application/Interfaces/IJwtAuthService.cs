using System.Security.Claims;
using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces
{
    public interface IJwtAuthService
    {
        string GenerateToken(AppUser user, string roleName);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

}
