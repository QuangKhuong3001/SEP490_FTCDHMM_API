using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOtpRepository _otpRepo;
        private readonly IMailService _mailService;
        private readonly IJwtAuthService _jwtService;
        private readonly IEmailTemplateService _emailTemplateService;

        public AuthService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOtpRepository otpRepo,
            IMailService mailService,
            IJwtAuthService jwtService,
            IEmailTemplateService emailTemplateService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _otpRepo = otpRepo;
            _mailService = mailService;
            _jwtService = jwtService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new AppException(AppResponseCode.EMAIL_ALREADY_EXISTS);

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
            };
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return (false, createResult.Errors.Select(e => e.Description));

            int otpLenght = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;
            string code = Generate.GenerateNumericOtp(otpLenght);
            string hashedCode = HashHelper.ComputeSha256Hash(code);

            var otp = new EmailOtp
            {
                UserId = user.Id,
                Code = hashedCode,
                Purpose = OtpPurpose.ConfirmAccountEmail,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(otpExpireMinutes)
            };
            await _otpRepo.AddAsync(otp);

            var localExpireTime = TimeZoneInfo.ConvertTimeFromUtc(otp.ExpiresAtUtc, TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "UserName", dto.Email },
                        { "OtpCode", code },
                        { "ExpireTime", localExpireTime.ToString("HH:mm dd/MM/yyyy") }
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(otp.Purpose.ToString(), placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);

            return (true, Array.Empty<string>());
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, dto.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                throw new AppException(AppResponseCode.ACCOUNT_LOCKED);
            }

            if (result.Succeeded)
            {
                if (!user.EmailConfirmed)
                    throw new AppException(AppResponseCode.EMAIL_NOT_CONFIRMED);

                var token = _jwtService.GenerateToken(user);
                return token;
            }
            else
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
            }
        }

        public async Task VerifyEmailOtpAsync(OtpVerifyDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var otp = await _otpRepo.GetLatestAsync(user.Id, OtpPurpose.ConfirmAccountEmail);
            if (otp == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            if (otp.IsDisabled)
                throw new AppException(AppResponseCode.OTP_INVALID);

            if (DateTime.UtcNow > otp.ExpiresAtUtc)
            {
                otp.IsDisabled = true;
                await _otpRepo.UpdateAsync(otp);
                throw new AppException(AppResponseCode.OTP_INVALID);
            }

            int maxOtpAttempts = OtpConstants.MaxAttempts;
            var hashedInput = HashHelper.ComputeSha256Hash(dto.Code);
            if (otp.Code != hashedInput)
            {
                otp.Attempts++;
                if (otp.Attempts >= maxOtpAttempts)
                {
                    otp.IsDisabled = true;
                }
                await _otpRepo.UpdateAsync(otp);
                throw new AppException(AppResponseCode.OTP_INVALID);
            }

            await _otpRepo.DeleteAsync(otp);

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task ResendOtpAsync(ResendOtpDto dto, OtpPurpose purpose)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.EmailConfirmed && purpose == OtpPurpose.ConfirmAccountEmail)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            int otpLength = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;

            string code = Generate.GenerateNumericOtp(otpLength);
            var hashedCode = HashHelper.ComputeSha256Hash(code);

            var otp = new EmailOtp
            {
                UserId = user.Id,
                Code = hashedCode,
                Purpose = purpose,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(otpExpireMinutes)
            };
            await _otpRepo.AddAsync(otp);

            var localExpireTime = TimeZoneInfo.ConvertTimeFromUtc(otp.ExpiresAtUtc, TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "UserName", dto.Email },
                        { "OtpCode", code },
                        { "ExpireTime", localExpireTime.ToString("HH:mm dd/MM/yyyy") }
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(purpose.ToString(), placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);
        }

        public async Task ForgotPasswordRequestAsync(ForgotPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return;

            int otpLength = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;

            string code = Generate.GenerateNumericOtp(otpLength);
            var hashedCode = HashHelper.ComputeSha256Hash(code);

            var otp = new EmailOtp
            {
                UserId = user.Id,
                Code = hashedCode,
                Purpose = OtpPurpose.ForgotPassword,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(otpExpireMinutes)
            };
            await _otpRepo.AddAsync(otp);

            var localExpireTime = TimeZoneInfo.ConvertTimeFromUtc(otp.ExpiresAtUtc, TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "UserName", dto.Email },
                        { "OtpCode", code },
                        { "ExpireTime", localExpireTime.ToString("HH:mm dd/MM/yyyy") }
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(otp.Purpose.ToString(), placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);
        }

        public async Task<string> VerifyOtpForPasswordResetAsync(VerifyOtpForPasswordResetDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var otp = await _otpRepo.GetLatestAsync(user.Id, OtpPurpose.ForgotPassword);
            if (otp == null || otp.IsDisabled || DateTime.UtcNow > otp.ExpiresAtUtc)
                throw new AppException(AppResponseCode.OTP_INVALID);

            int maxOtpAttempts = OtpConstants.MaxAttempts;
            var hashedInput = HashHelper.ComputeSha256Hash(dto.Code);

            if (otp.Code != hashedInput)
            {
                otp.Attempts++;
                if (otp.Attempts >= maxOtpAttempts)
                    otp.IsDisabled = true;

                await _otpRepo.UpdateAsync(otp);
                throw new AppException(AppResponseCode.OTP_INVALID);
            }

            await _otpRepo.DeleteAsync(otp);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return token;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (await _userManager.CheckPasswordAsync(user, dto.NewPassword))
                throw new AppException(AppResponseCode.PASSWORD_CANNOT_BE_SAME_AS_OLD);

            var resetResult = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!resetResult.Succeeded)
                return (false, resetResult.Errors.Select(e => e.Description));

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var changePassResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!changePassResult.Succeeded)
            {
                return (false, changePassResult.Errors.Select(e => e.Description));
            }

            return (true, Array.Empty<string>());
        }
    }
}
