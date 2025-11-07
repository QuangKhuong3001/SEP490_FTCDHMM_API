using System.Data;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
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
        private readonly IRoleRepository _roleRepository;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOtpRepository _otpRepo;
        private readonly IMailService _mailService;
        private readonly IJwtAuthService _jwtService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IGoogleProvisioningService _googleProvisioningService;

        public AuthService(UserManager<AppUser> userManager,
            IRoleRepository roleRepository,
            SignInManager<AppUser> signInManager,
            IOtpRepository otpRepo,
            IMailService mailService,
            IJwtAuthService jwtService,
            IGoogleProvisioningService googleProvisioningService,
            IGoogleAuthService googleAuthService,
            IEmailTemplateService emailTemplateService)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _signInManager = signInManager;
            _otpRepo = otpRepo;
            _mailService = mailService;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
            _googleProvisioningService = googleProvisioningService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> Register(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new AppException(AppResponseCode.EMAIL_ALREADY_EXISTS);

            var customerRole = await _roleRepository.FindByNameAsync(RoleValue.Customer.Name);

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                RoleId = customerRole!.Id,
                Gender = Gender.From(dto.Gender),
                DateOfBirth = dto.DateOfBirth
            };
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return (false, createResult.Errors.Select(e => e.Description));

            string otpCode = Generate.GenerateNumericOtp(OtpConstants.Length);
            string hashedCode = HashHelper.ComputeSha256Hash(otpCode);

            var otp = new EmailOtp
            {
                SentToId = user.Id,
                Code = hashedCode,
                Purpose = OtpPurpose.VerifyAccountEmail,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(OtpConstants.ExpireMinutes)
            };
            await _otpRepo.AddAsync(otp);

            var localExpireTime = TimeZoneInfo.ConvertTimeFromUtc(otp.ExpiresAtUtc, TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "UserName", dto.Email },
                        { "OtpCode", otpCode },
                        { "ExpireTime", localExpireTime.ToString("HH:mm dd/MM/yyyy") }
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.VerifyAccountEmail, placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);

            return (true, Array.Empty<string>());
        }

        public async Task<string> Login(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (await _userManager.IsLockedOutAsync(user))
                throw new AppException(AppResponseCode.ACCOUNT_LOCKED);

            if (!user.EmailConfirmed)
                throw new AppException(AppResponseCode.EMAIL_NOT_CONFIRMED);

            var role = await _roleRepository.GetRoleWithPermissionsAsync(user.RoleId);

            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var token = _jwtService.GenerateToken(user, role);

            return token;
        }


        public async Task VerifyEmailOtp(OtpVerifyRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var otp = await _otpRepo.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail);
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

        public async Task ResendOtp(ResendOtpRequest dto, OtpPurpose purpose)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.EmailConfirmed && purpose == OtpPurpose.VerifyAccountEmail)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            var oldOtp = await _otpRepo.GetLatestAsync(user.Id, purpose);
            if (oldOtp != null)
            {
                await _otpRepo.DeleteAsync(oldOtp);
            }

            int otpLength = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;

            string code = Generate.GenerateNumericOtp(otpLength);
            var hashedCode = HashHelper.ComputeSha256Hash(code);

            var otp = new EmailOtp
            {
                SentToId = user.Id,
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

            if (purpose == OtpPurpose.VerifyAccountEmail)
            {
                var emailTemplateType = EmailTemplateType.VerifyAccountEmail;
                var htmlBody = await _emailTemplateService.RenderTemplateAsync(emailTemplateType, placeholders);
                await _mailService.SendEmailAsync(dto.Email, htmlBody);

            }
            else if (purpose == OtpPurpose.ForgotPassword)
            {
                var emailTemplateType = EmailTemplateType.ForgotPassword;
                var htmlBody = await _emailTemplateService.RenderTemplateAsync(emailTemplateType, placeholders);
                await _mailService.SendEmailAsync(dto.Email, htmlBody);

            }

        }

        public async Task ForgotPasswordRequest(ForgotPasswordRequest dto)
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
                SentToId = user.Id,
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

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.ForgotPassword, placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);
        }

        public async Task<string> VerifyOtpForPasswordReset(VerifyOtpForPasswordResetRequest dto)
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

        public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordWithToken(ResetPasswordWithTokenDto dto)
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

        public async Task<(bool Success, IEnumerable<string> Errors)> ChangePassword(string userId, ChangePasswordRequest dto)
        {
            if (dto.CurrentPassword == dto.NewPassword)
                throw new AppException(AppResponseCode.INVALID_ACTION);

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

        public async Task<string> GoogleLoginWithCodeAsync(GoogleCodeLoginRequest dto)
        {
            var tokens = await _googleAuthService.ExchangeCodeForTokenAsync(new GoogleCodeLoginRequest
            {
                Code = dto.Code,
                CodeVerifier = dto.CodeVerifier
            });
            if (tokens == null) throw new AppException(AppResponseCode.SERVICE_NOT_AVAILABLE);

            var payload = await _googleAuthService.ValidateIdTokenAsync(tokens.IdToken);

            var info = await _googleAuthService.FetchUserInfoWithPeopleApiAsync(tokens.AccessToken);

            var user = await _googleProvisioningService.FindOrProvisionFromGoogleAsync(new GoogleProvisionRequest
            {
                Payload = payload,
                UserInfo = info,
                GoogleRefreshToken = tokens.RefreshToken
            });

            return _jwtService.GenerateToken(user, user.Role);
        }

        public async Task<string> GoogleLoginWithIdTokenAsync(GoogleIdTokenLoginRequest dto)
        {
            var payload = await _googleAuthService.ValidateIdTokenAsync(dto.IdToken);

            var user = await _googleProvisioningService.FindOrProvisionFromGoogleAsync(new GoogleProvisionRequest
            {
                Payload = payload,
                UserInfo = null,
                GoogleRefreshToken = null
            });

            return _jwtService.GenerateToken(user, user.Role);
        }
    }
}
