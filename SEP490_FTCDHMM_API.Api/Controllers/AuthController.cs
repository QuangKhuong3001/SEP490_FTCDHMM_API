using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Attributes;
using SEP490_FTCDHMM_API.Api.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Api.Dtos.GoogleAuthDtos;
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
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.RegisterDto>(dto);

            var (success, errors) = await _authService.Register(appDto);
            if (!success) return BadRequest(new { success = false, errors });
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp(OtpVerifyRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.OtpVerifyRequest>(dto);

            await _authService.VerifyEmailOtp(appDto);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(ResendOtpRequest dto, [FromQuery] string purpose = "VERIFYACCOUNTEMAIL")
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ResendOtpRequest>(dto);

            var purposeKey = (purpose ?? string.Empty).Trim().ToUpperInvariant();
            OtpPurpose parsedPurpose = OtpPurpose.From(purpose!);

            await _authService.ResendOtp(appDto, parsedPurpose);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.LoginRequest>(dto);

            var token = await _authService.Login(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ForgotPasswordRequest>(dto);

            await _authService.ForgotPasswordRequest(appDto);
            return Ok();
        }

        [DisallowAuthenticated]
        [HttpPost("verify-otp-for-password-reset")]
        public async Task<IActionResult> VerifyOtpForPasswordReset(VerifyOtpForPasswordResetRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.VerifyOtpForPasswordResetRequest>(dto);

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
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.AuthDTOs.ChangePasswordRequest>(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (Success, Errors) = await _authService.ChangePassword(userId!, appDto);

            if (!Success)
                return BadRequest(new { errors = Errors });

            return Ok();
        }


        [DisallowAuthenticated]
        [HttpPost("google/code")]
        public async Task<IActionResult> GoogleLoginWithCode(GoogleCodeLoginRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.GoogleAuthDtos.GoogleCodeLoginRequest>(dto);

            var token = await _authService.GoogleLoginWithCodeAsync(appDto);
            return Ok(new { token });
        }

        [DisallowAuthenticated]
        [HttpPost("google/id-token")]
        public async Task<IActionResult> GoogleLoginWithIdToken(GoogleIdTokenLoginRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.GoogleAuthDtos.GoogleIdTokenLoginRequest>(dto);

            var token = await _authService.GoogleLoginWithIdTokenAsync(appDto);
            return Ok(new { token });
        }
    }
}