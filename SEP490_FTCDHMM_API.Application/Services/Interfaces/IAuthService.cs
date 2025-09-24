using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task VerifyEmailOtpAsync(OtpVerifyDto dto);
        Task ResendOtpAsync(ResendOtpDto dto, OtpPurpose purpose);
        Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto dto);
        Task<string> VerifyOtpForPasswordResetAsync(VerifyOtpForPasswordResetDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDto dto);
        Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, ChangePasswordDto dto);

    }

}
