using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> Register(RegisterDto dto);
        Task<string?> Login(LoginDto dto);
        Task VerifyEmailOtp(OtpVerifyDto dto);
        Task ResendOtp(ResendOtpDto dto, OtpPurpose purpose);
        Task ForgotPasswordRequest(ForgotPasswordRequestDto dto);
        Task<string> VerifyOtpForPasswordReset(VerifyOtpForPasswordResetDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithToken(ResetPasswordWithTokenDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangePassword(string userId, ChangePasswordDto dto);

    }

}
