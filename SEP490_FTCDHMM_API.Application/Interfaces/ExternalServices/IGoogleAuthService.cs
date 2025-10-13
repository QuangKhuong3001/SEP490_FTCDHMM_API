using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;

namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface IGoogleAuthService
    {
        Task<GoogleTokenResponse?> ExchangeCodeForTokenAsync(GoogleCodeLoginRequest dto);
        Task<GoogleTokenPayload> ValidateIdTokenAsync(string idToken);
        Task<GoogleUserInfo?> FetchUserInfoWithPeopleApiAsync(string accessToken);
    }
}
