using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Attributes;
using SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
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
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.RegisterDto>(dto);

            var (success, errors) = await _authService.Register(appDto);
            if (!success) return BadRequest(new { success = false, errors });
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp(OtpVerifyDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.OtpVerifyDto>(dto);

            await _authService.VerifyEmailOtp(appDto);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(ResendOtpDto dto, [FromQuery] string purpose = "VERIFYACCOUNTEMAIL")
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ResendOtpDto>(dto);

            var purposeKey = (purpose ?? string.Empty).Trim().ToUpperInvariant();
            OtpPurpose parsedPurpose = OtpPurpose.From(purpose!);

            await _authService.ResendOtp(appDto, parsedPurpose);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.LoginDto>(dto);

            var token = await _authService.Login(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ForgotPasswordRequestDto>(dto);

            await _authService.ForgotPasswordRequest(appDto);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("verify-otp-for-password-reset")]
        public async Task<IActionResult> VerifyOtpForPasswordReset(VerifyOtpForPasswordResetDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.VerifyOtpForPasswordResetDto>(dto);

            var token = await _authService.VerifyOtpForPasswordReset(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordWithTokenDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ResetPasswordWithTokenDto>(dto);

            var (success, errors) = await _authService.ResetPasswordWithToken(appDto);
            if (!success)
                return BadRequest(new { success = false, errors });
            return Ok();
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ChangePasswordDto>(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (Success, Errors) = await _authService.ChangePassword(userId!, appDto);

            if (!Success)
                return BadRequest(new { errors = Errors });

            return Ok();
        }
    }

}
