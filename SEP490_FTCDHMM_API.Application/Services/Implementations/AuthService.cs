using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.AuthDTOs;
using SEP490_FTCDHMM_API.Application.Dtos.GoogleAuthDtos;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly INotificationCommandService _notificationCommandServices;
        private readonly IRoleRepository _roleRepository;
        private readonly IOtpRepository _otpRepo;
        private readonly IMailService _mailService;
        private readonly IJwtAuthService _jwtService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IGoogleProvisioningService _googleProvisioningService;

        public AuthService(UserManager<AppUser> userManager,
            IRoleRepository roleRepository,
            IOtpRepository otpRepo,
            INotificationCommandService notificationCommandService,
            IMailService mailService,
            IJwtAuthService jwtService,
            IGoogleProvisioningService googleProvisioningService,
            IGoogleAuthService googleAuthService,
            IEmailTemplateService emailTemplateService)
        {
            _userManager = userManager;
            _notificationCommandServices = notificationCommandService;
            _roleRepository = roleRepository;
            _otpRepo = otpRepo;
            _mailService = mailService;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
            _googleProvisioningService = googleProvisioningService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequest dto)
        {
            var age = AgeCalculator.Calculate(dto.DateOfBirth);
            if (age < AuthConstants.MIN_REGISTER_AGE || age > AuthConstants.MAX_REGISTER_AGE)
                throw new AppException(AppResponseCode.INVALID_ACTION, $"Tuổi phải nằm trong khoảng {AuthConstants.MIN_REGISTER_AGE} đến {AuthConstants.MAX_REGISTER_AGE} tuổi");

            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                if (!existing.EmailConfirmed)
                    throw new AppException(AppResponseCode.EXISTS, "Email đã được đăng ký nhưng chưa được xác thực");

                throw new AppException(AppResponseCode.EXISTS, "Email đã được sử dụng");
            }

            var customerRole = await _roleRepository.FindByNameAsync(RoleValue.Customer.Name);

            var userName = UsernameHelper.ExtractUserName(dto.Email);

            var user = new AppUser
            {
                UserName = userName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = customerRole!.Id,
                Gender = Gender.From(dto.Gender),
                DateOfBirth = dto.DateOfBirth
            };

            var createResult = default(IdentityResult);

            while (true)
            {
                try
                {
                    user.UserName = UsernameHelper.IncrementUserName(userName);
                    createResult = await _userManager.CreateAsync(user, dto.Password);

                    if (createResult.Succeeded)
                        break;

                    if (createResult.Errors.Any(e => e.Code == "DuplicateUserName"))
                    {
                        continue;
                    }

                    return (false, createResult.Errors.Select(e => e.Description));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("IX_AspNetUsers_UserName") == true)
                    {
                        continue;
                    }
                    throw;
                }
            }

            string otpCode = Generate.GenerateRandomNumberic(OtpConstants.Length);
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

            await _mailService.SendEmailAsync(dto.Email, htmlBody, "Chào mừng bạn đến với FitFood Tracker");
            await _notificationCommandServices.CreateAndSendNotificationAsync(null, user.Id, NotificationType.System, null);
            return (true, Array.Empty<string>());
        }

        public async Task<string> LoginAsync(LoginRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user!, dto.Password);
            if (!isPasswordValid)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (await _userManager.IsLockedOutAsync(user!))
                throw new AppException(AppResponseCode.ACCOUNT_LOCKED);

            if (!user!.EmailConfirmed)
                throw new AppException(AppResponseCode.EMAIL_NOT_CONFIRMED);

            var role = await _roleRepository.GetRoleWithPermissionsAsync(user.RoleId);

            if (role == null)
                throw new AppException(AppResponseCode.NOT_FOUND);

            var token = _jwtService.GenerateToken(user, role);

            return token;
        }

        public async Task VerifyEmailOtpAsync(OtpVerifyRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var otp = await _otpRepo.GetLatestAsync(user!.Id, OtpPurpose.VerifyAccountEmail);
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

        public async Task ResendVerifyAccountEmailOtpAsync(ResendOtpRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user!.EmailConfirmed)
            {
                throw new AppException(AppResponseCode.INVALID_ACTION);
            }

            var oldOtp = await _otpRepo.GetLatestAsync(user.Id, OtpPurpose.VerifyAccountEmail);
            if (oldOtp != null)
            {
                await _otpRepo.DeleteAsync(oldOtp);
            }

            int otpLength = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;

            string code = Generate.GenerateRandomNumberic(otpLength);
            var hashedCode = HashHelper.ComputeSha256Hash(code);

            var otp = new EmailOtp
            {
                SentToId = user.Id,
                Code = hashedCode,
                Purpose = OtpPurpose.VerifyAccountEmail,
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

            var emailTemplateType = EmailTemplateType.VerifyAccountEmail;
            var htmlBody = await _emailTemplateService.RenderTemplateAsync(emailTemplateType, placeholders);
            await _mailService.SendEmailAsync(dto.Email, htmlBody, "Xác thực tài khoản FitFood Tracker");

        }

        public async Task ForgotPasswordRequestAsync(ForgotPasswordRequest dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return;

            int otpLength = OtpConstants.Length;
            int otpExpireMinutes = OtpConstants.ExpireMinutes;

            string code = Generate.GenerateRandomNumberic(otpLength);
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

            await _mailService.SendEmailAsync(dto.Email, htmlBody, "Đặt lại mật khẩu tài khoản FitFood Tracker");
        }

        public async Task<string> VerifyOtpForPasswordResetAsync(VerifyOtpForPasswordResetRequest dto)
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

        public async Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, ChangePasswordRequest dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (await _userManager.CheckPasswordAsync(user, dto.NewPassword))
                throw new AppException(AppResponseCode.PASSWORD_CANNOT_BE_SAME_AS_OLD);

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
