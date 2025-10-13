using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IS3ImageService _s3ImageService;
        private readonly IUserFollowRepository _userFollowRepository;

        public UserService(IUserRepository userRepository,
            UserManager<AppUser> userManager,
            IMapper mapper,
            IRoleRepository roleRepository,
            IOtpRepository otpRepository,
            IMailService mailService,
            IEmailTemplateService emailTemplateService,
            IS3ImageService s3ImageService,
            IUserFollowRepository userFollowRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _otpRepository = otpRepository;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
            _s3ImageService = s3ImageService;
            _userFollowRepository = userFollowRepository;
        }

        public async Task<PagedResult<UserResponse>> GetCustomerList(PaginationParams pagination)
        {
            var (customers, totalCount) = await _userRepository.GetPagedAsync(
                pagination.Page, pagination.PageSize,
                u => u.Role.Name == RoleValue.Customer.Name,
                q => q.OrderBy(u => u.CreatedAtUtc));

            var result = _mapper.Map<List<UserResponse>>(customers);

            return new PagedResult<UserResponse>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<LockResponse> LockCustomerAccount(LockRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            user.LockoutEnd = DateTime.UtcNow.AddDays(dto.Day);

            await _userRepository.UpdateAsync(user);

            return new LockResponse
            {
                Email = user.Email!,
                LockoutEnd = user.LockoutEnd
            };
        }

        public async Task<UnlockResponse> UnLockCustomerAccount(UnlockRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.LockoutEnd <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            return new UnlockResponse
            {
                Email = user.Email!,
            };
        }

        public async Task<PagedResult<UserResponse>> GetModeratorList(PaginationParams pagination)
        {
            var (modetators, totalCount) = await _userRepository.GetPagedAsync(
                pagination.Page, pagination.PageSize,
                u => u.Role.Name == RoleValue.Moderator.Name,
                q => q.OrderBy(u => u.CreatedAtUtc));

            var result = _mapper.Map<List<UserResponse>>(modetators);

            return new PagedResult<UserResponse>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }
        public async Task<LockResponse> LockModeratorAccount(LockRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            user.LockoutEnd = DateTime.UtcNow.AddDays(dto.Day);

            await _userRepository.UpdateAsync(user);

            return new LockResponse
            {
                Email = user.Email!,
                LockoutEnd = user.LockoutEnd
            };
        }
        public async Task<UnlockResponse> UnLockModeratorAccount(UnlockRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.LockoutEnd <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);

            return new UnlockResponse
            {
                Email = user.Email!,
            };
        }

        public async Task<CreateModeratorAccountResponse> CreateModeratorAccount(CreateModeratorAccountRequest dto)
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
                return new CreateModeratorAccountResponse
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

            return new CreateModeratorAccountResponse
            {
                Success = true,
            };
        }
        public async Task<ProfileDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId, u => u.Role, u => u.Avatar!);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var profile = _mapper.Map<ProfileDto>(user);
            profile.Avatar = _s3ImageService.GeneratePreSignedUrl(user.Avatar?.Key ?? null);
            return profile;
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(userId, u => u.Role);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = Gender.From(dto.Gender);
            user.UpdatedAtUtc = DateTime.UtcNow;

            if (dto.Avatar != null && dto.Avatar.Length > 0)
            {
                var uploadedImage = await _s3ImageService.UploadImageAsync(dto.Avatar, StorageFolder.AVATARS, user);

                if (user.AvatarId.HasValue)
                {
                    await _s3ImageService.DeleteImageAsync(user.AvatarId.Value);
                }

                user.AvatarId = uploadedImage.Id;
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task FollowUserAsync(Guid followerId, Guid followeeId)
        {
            if (followerId == followeeId)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var exist = await _userFollowRepository.ExistsAsync(u => u.FollowerId == followerId && u.FolloweeId == followeeId);

            if (exist)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            var follow = new UserFollow
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userFollowRepository.AddAsync(follow);
        }


        public async Task UnfollowUserAsync(Guid followerId, Guid followeeId)
        {
            var follows = await _userFollowRepository.GetAllAsync(
                f => f.FollowerId == followerId && f.FolloweeId == followeeId
            );

            var follow = follows.FirstOrDefault();
            if (follow == null)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            await _userFollowRepository.DeleteAsync(follow);
        }


        public async Task<List<UserDto>> GetFollowersAsync(Guid userId)
        {
            var followers = await _userFollowRepository.GetAllAsync(
                u => u.FolloweeId == userId,
                u => u.Follower
            );


            var followerUsers = followers.Select(f => f.Follower).ToList();

            return _mapper.Map<List<UserDto>>(followerUsers);
        }

        public async Task<List<UserDto>> GetFollowingAsync(Guid userId)
        {
            var followings = await _userFollowRepository.GetAllAsync(
                u => u.FollowerId == userId,
                u => u.Followee
            );

            var followingUsers = followings.Select(f => f.Followee).ToList();

            return _mapper.Map<List<UserDto>>(followingUsers);
        }

    }
}
