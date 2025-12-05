using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequest dto);
        Task<string> LoginAsync(LoginRequest dto);
        Task VerifyEmailOtpAsync(OtpVerifyRequest dto);
        Task ResendVerifyAccountEmailOtpAsync(ResendOtpRequest dto);
        Task ForgotPasswordRequestAsync(ForgotPasswordRequest dto);
        Task<string> VerifyOtpForPasswordResetAsync(VerifyOtpForPasswordResetRequest dto);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, ChangePasswordRequest dto);
        Task<string> GoogleLoginWithCodeAsync(GoogleCodeLoginRequest dto);
        Task<string> GoogleLoginWithIdTokenAsync(GoogleIdTokenLoginRequest dto);

    }

}
