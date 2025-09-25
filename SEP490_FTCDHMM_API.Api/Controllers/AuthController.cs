using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Attributes;
using SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using APIDtos = SEP490_FTCDHMM_API.Api.Dtos;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [DisallowAuthenticated]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.RegisterDto>(dto);

            var (success, errors) = await _authService.RegisterAsync(appDto);
            if (!success) return BadRequest(new { success = false, errors });
            return Ok(new { message = "Registered. Check your email for OTP." });
        }

        [DisallowAuthenticated]
        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] APIDtos.AuthDTOs.OtpVerifyDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.OtpVerifyDto>(dto);

            await _authService.VerifyEmailOtpAsync(appDto);
            return Ok(new { message = "Email confirmed." });
        }

        [DisallowAuthenticated]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto, [FromQuery] string purpose = "ConfirmEmail")
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ResendOtpDto>(dto);

            var purposeKey = (purpose ?? string.Empty).Trim().ToLowerInvariant();
            OtpPurpose parsedPurpose = purposeKey switch
            {
                "confirm" or "confirmemail" => OtpPurpose.ConfirmAccountEmail,
                "forgot" or "forgotpassword" or "reset" => OtpPurpose.ForgotPassword,
                _ => throw new AppException(AppResponseCode.INVALID_ACTION),
            };

            await _authService.ResendOtpAsync(appDto, parsedPurpose);
            return Ok(new { message = "OTP resent." });
        }

        [DisallowAuthenticated]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.LoginDto>(dto);

            var token = await _authService.LoginAsync(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ForgotPasswordRequestDto>(dto);

            await _authService.ForgotPasswordRequestAsync(appDto);
            return Ok(new { message = "If the email exists, an OTP has been sent." });
        }

        [DisallowAuthenticated]
        [HttpPost("verify-otp-for-password-reset")]
        public async Task<IActionResult> VerifyOtpForPasswordReset([FromBody] VerifyOtpForPasswordResetDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.VerifyOtpForPasswordResetDto>(dto);

            var token = await _authService.VerifyOtpForPasswordResetAsync(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithTokenDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ResetPasswordWithTokenDto>(dto);

            var (success, errors) = await _authService.ResetPasswordWithTokenAsync(appDto);
            if (!success) return BadRequest(new { success = false, errors });
            return Ok(new { message = "Password has been reset." });
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ChangePasswordDto>(dto);

            var userId = User.FindFirstValue(AppClaimTypes.UserId);

            var result = await _authService.ChangePasswordAsync(userId!, appDto);

            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Password changed successfully." });
        }
    }

}
