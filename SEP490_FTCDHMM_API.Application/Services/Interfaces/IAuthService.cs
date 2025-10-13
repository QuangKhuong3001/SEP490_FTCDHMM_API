using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> Register(RegisterDto dto);
        Task<string> Login(LoginRequest dto);
        Task VerifyEmailOtp(OtpVerifyRequest dto);
        Task ResendOtp(ResendOtpRequest dto, OtpPurpose purpose);
        Task ForgotPasswordRequest(ForgotPasswordRequest dto);
        Task<string> VerifyOtpForPasswordReset(VerifyOtpForPasswordResetRequest dto);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithToken(ResetPasswordWithTokenDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangePassword(string userId, ChangePasswordRequest dto);
        Task<string> GoogleLoginWithCodeAsync(GoogleCodeLoginRequest dto);
        Task<string> GoogleLoginWithIdTokenAsync(GoogleIdTokenLoginRequest dto);

    }

}
