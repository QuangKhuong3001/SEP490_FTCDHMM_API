using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;
using SEP490_FTCDHMM_API.Shared.Utils;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public UserService(IUserRepository userRepository,
            UserManager<AppUser> userManager,
            IMapper mapper,
            IRoleRepository roleRepository,
            IOtpRepository otpRepository,
            IMailService mailService,
            IEmailTemplateService emailTemplateService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _otpRepository = otpRepository;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
        }

        public async Task<PagedResult<UserDto>> GetCustomerList(PaginationParams pagination)
        {
            var (customers, totalCount) = await _userRepository.GetPagedAsync(
                pagination.Page, pagination.PageSize,
                u => u.Role.Name == RoleValue.Customer.Name,
                q => q.OrderBy(u => u.CreatedAtUtc));

            var result = _mapper.Map<List<UserDto>>(customers);

            return new PagedResult<UserDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<LockResultDto> LockCustomerAccount(LockRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != RoleValue.Customer.Name)
                throw new AppException(AppResponseCode.NO_PERMISSION);

            user.LockoutEnd = DateTime.UtcNow.AddDays(dto.Day);

            await _userRepository.UpdateAsync(user);

            return new LockResultDto
            {
                Email = user.Email!,
                LockoutEnd = user.LockoutEnd
            };
        }

        public async Task<UnlockResultDto> UnLockCustomerAccount(UnlockRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != RoleValue.Customer.Name)
                throw new AppException(AppResponseCode.NO_PERMISSION);

            if (user.LockoutEnd <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            return new UnlockResultDto
            {
                Email = user.Email!,
            };
        }

        public async Task<PagedResult<UserDto>> GetModeratorList(PaginationParams pagination)
        {
            var (modetators, totalCount) = await _userRepository.GetPagedAsync(
                pagination.Page, pagination.PageSize,
                u => u.Role.Name == RoleValue.Moderator.Name,
                q => q.OrderBy(u => u.CreatedAtUtc));

            var result = _mapper.Map<List<UserDto>>(modetators);

            return new PagedResult<UserDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
        public async Task<LockResultDto> LockModeratorAccount(LockRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != RoleValue.Moderator.Name)
                throw new AppException(AppResponseCode.NO_PERMISSION);

            user.LockoutEnd = DateTime.UtcNow.AddDays(dto.Day);

            await _userRepository.UpdateAsync(user);

            return new LockResultDto
            {
                Email = user.Email!,
                LockoutEnd = user.LockoutEnd
            };
        }
        public async Task<UnlockResultDto> UnLockModeratorAccount(UnlockRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != RoleValue.Moderator.Name)
                throw new AppException(AppResponseCode.NO_PERMISSION);

            if (user.LockoutEnd <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            return new UnlockResultDto
            {
                Email = user.Email!,
            };
        }

        public async Task<CreateModeratorAccountResult> CreateModeratorAccount(CreateModeratorAccountDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new AppException(AppResponseCode.EMAIL_ALREADY_EXISTS);

            var moderatorRole = await _roleRepository.FindByNameAsync(RoleValue.Moderator.Name);

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = ModeratorAccountConstants.FirstName,
                LastName = ModeratorAccountConstants.LastName,
                RoleId = moderatorRole!.Id,
            };

            string password = Generate.GeneratePassword(ModeratorAccountConstants.PasswordLength);

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return new CreateModeratorAccountResult
                {
                    Success = false,
                    Errors = createResult.Errors.Select(e => e.Description)
                };

            string otpCode = Generate.GenerateNumericOtp(OtpConstants.Length);
            string hashedCode = HashHelper.ComputeSha256Hash(otpCode);

            var otp = new EmailOtp
            {
                SentToId = user.Id,
                Code = hashedCode,
                Purpose = OtpPurpose.VerifyAccountEmail,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(ModeratorAccountConstants.OtpExpireDays)
            };
            await _otpRepository.AddAsync(otp);

            var localExpireTime = TimeZoneInfo.ConvertTimeFromUtc(otp.ExpiresAtUtc, TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "UserName", dto.Email },
                        { "OtpCode", otpCode },
                        { "Password", password },
                        { "ExpireTime", localExpireTime.ToString("HH:mm dd/MM/yyyy") }
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.ModeratorCreated, placeholders);

            await _mailService.SendEmailAsync(dto.Email, htmlBody);

            return new CreateModeratorAccountResult
            {
                Success = true,
            };
        }
        public async Task<ProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var profile = _mapper.Map<ProfileDto>(user);
            return profile;
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = Gender.From(dto.Gender);
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

    }
}
