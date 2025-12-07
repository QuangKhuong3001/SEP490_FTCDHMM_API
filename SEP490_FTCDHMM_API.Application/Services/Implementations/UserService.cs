using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos.Mention;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Interfaces.SystemServices;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.Services;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IMailService _mailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IS3ImageService _s3ImageService;
        private readonly IUserFollowRepository _userFollowRepository;

        public UserService(IUserRepository userRepository,
            IMapper mapper,
            IRoleRepository roleRepository,
            IMailService mailService,
            IEmailTemplateService emailTemplateService,
            IS3ImageService s3ImageService,
            IUserFollowRepository userFollowRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _mailService = mailService;
            _emailTemplateService = emailTemplateService;
            _s3ImageService = s3ImageService;
            _userFollowRepository = userFollowRepository;
        }

        public async Task<PagedResult<UserResponse>> GetUserListAsync(UserFilterRequest request)
        {
            var (customers, totalCount) = await _userRepository.GetPagedAsync(
                request.PaginationParams.PageNumber, request.PaginationParams.PageSize,
                u => (string.IsNullOrEmpty(request.Keyword) ||
                      u.FirstName.Contains(request.Keyword!) ||
                      u.LastName.Contains(request.Keyword!) ||
                      u.Email!.Contains(request.Keyword!))
                      && (!request.RoleId.HasValue || u.RoleId == request.RoleId.Value),
                q => q.OrderBy(u => u.CreatedAtUtc),
                include: i => i.Include(u => u.Role)
                );

            var result = _mapper.Map<List<UserResponse>>(customers);

            return new PagedResult<UserResponse>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = request.PaginationParams.PageNumber,
                PageSize = request.PaginationParams.PageSize
            };
        }

        public async Task<LockResponse> LockUserAccountAsync(Guid userId, LockRequest dto)
        {
            var user = await _userRepository.GetByIdAsync(userId,
                include: u => u.Include(u => u.Role));

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = DateTime.UtcNow.AddDays(dto.Day);
            user.LockReason = dto.Reason;

            var localLockoutEnd = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(dto.Day), TimeZoneInfo.Local);
            var placeholders = new Dictionary<string, string>
                    {
                        { "LockoutEnd", localLockoutEnd.ToString() },
                        { "LockReason", dto.Reason },
                    };

            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.LockAccount, placeholders);

            await _mailService.SendEmailAsync(user.Email!, htmlBody, "Thông báo khóa tài khoản");

            await _userRepository.UpdateAsync(user);

            return new LockResponse
            {
                Email = user.Email!,
                LockoutEnd = user.LockoutEnd
            };
        }

        public async Task<UnlockResponse> UnLockUserAccountAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId,
                include: u => u.Include(u => u.Role));
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.LockoutEnd <= DateTime.UtcNow)
                throw new AppException(AppResponseCode.INVALID_ACTION);

            user.LockoutEnd = null;
            user.LockReason = null;

            var placeholders = new Dictionary<string, string> { };
            var htmlBody = await _emailTemplateService.RenderTemplateAsync(EmailTemplateType.UnlockAccount, placeholders);
            await _mailService.SendEmailAsync(user.Email!, htmlBody, "Thông báo mở khóa tài khoản");

            await _userRepository.UpdateAsync(user);

            return new UnlockResponse
            {
                Email = user.Email!,
            };
        }

        public async Task<ProfileResponse> GetMyProfileAsync(Guid userId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(
                orderByDescendingKeySelector: u => u.UserName,
                predicate: u => u.Id == userId,
                include: i => i.Include(u => u.Role)
                                .Include(u => u.Followers)
                                .Include(u => u.Following)
                                .Include(u => u.Avatar));

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var profile = _mapper.Map<ProfileResponse>(user);

            var followersCount = user.Followers.Count(f => f.FolloweeId == user.Id);
            profile.FollowersCount = followersCount;

            var followingCount = user.Following.Count(f => f.FollowerId == user.Id);
            profile.FollowingCount = followingCount;

            return profile;

        }
        public async Task<ProfileResponse> GetProfileAsync(string? username, Guid? currentUserId = null)
        {
            var user = await _userRepository.FirstOrDefaultAsync(
                orderByDescendingKeySelector: u => u.UserName,
                predicate: u => u.UserName == username,
                include: i => i.Include(u => u.Role)
                                .Include(u => u.Followers)
                                .Include(u => u.Following)
                                .Include(u => u.Avatar));

            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            var profile = _mapper.Map<ProfileResponse>(user);

            var followersCount = user.Followers.Count(f => f.FolloweeId == user.Id);
            profile.FollowersCount = followersCount;

            var followingCount = user.Following.Count(f => f.FollowerId == user.Id);
            profile.FollowingCount = followingCount;

            if (currentUserId.HasValue && currentUserId.Value != user.Id)
            {
                var isFollowing = await _userFollowRepository.ExistsAsync(
                    f => f.FollowerId == currentUserId.Value && f.FolloweeId == user.Id
                );
                profile.IsFollowing = isFollowing;
            }
            else
            {
                profile.IsFollowing = false;
            }

            return profile;
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto)
        {
            var age = AgeCalculator.Calculate(dto.DateOfBirth);
            if (age < AuthConstants.MIN_REGISTER_AGE || age > AuthConstants.MAX_REGISTER_AGE)
                throw new AppException(AppResponseCode.INVALID_ACTION, $"Tuổi phải nằm trong khoảng {AuthConstants.MIN_REGISTER_AGE} đến {AuthConstants.MAX_REGISTER_AGE} tuổi");

            var user = await _userRepository.GetByIdAsync(userId,
                include: u => u.Include(u => u.Role));

            user!.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Gender = Gender.From(dto.Gender);
            user.DateOfBirth = dto.DateOfBirth;
            user.Bio = dto.Bio;
            user.Address = dto.Address;

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

            var followee = await _userRepository.GetByIdAsync(followeeId);
            if (followee == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

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


        public async Task<IEnumerable<UserInteractionResponse>> GetFollowersAsync(Guid userId)
        {
            var followers = await _userFollowRepository.GetAllAsync(
                u => u.FolloweeId == userId,
                include: i => i.Include(u => u.Follower).ThenInclude(u => u.Avatar));

            var followerUsers = followers
                .Select(f => f.Follower)
                .OrderBy(f => f.FirstName)
                .ThenBy(f => f.LastName)
                .ToList();

            return _mapper.Map<IEnumerable<UserInteractionResponse>>(followerUsers);
        }

        public async Task<IEnumerable<UserInteractionResponse>> GetFollowingAsync(Guid userId)
        {
            var followings = await _userFollowRepository.GetAllAsync(
                u => u.FollowerId == userId,
                include: i => i.Include(u => u.Followee).ThenInclude(u => u.Avatar));

            var followingUsers = followings
                .Select(f => f.Followee)
                .OrderBy(f => f.FirstName)
                .ThenBy(f => f.LastName)
                .ToList();

            return _mapper.Map<IEnumerable<UserInteractionResponse>>(followingUsers);
        }

        public async Task<ActivityLevel> GetActivityLevelAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
            }

            return user.ActivityLevel;
        }

        public async Task ChangeActivityLevelAsync(Guid userId, ChangeActivityLevelRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);
            }

            user.ActivityLevel = ActivityLevel.From(request.ActivityLevel);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangeRoleAsync(Guid userId, ChangeRoleRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION, "Người dùng không tồn tại");
            }

            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null || !role.IsActive)
            {
                throw new AppException(AppResponseCode.NOT_FOUND, "Vai trò không tồn tại");
            }

            if (role.Name == RoleConstants.Admin)
                throw new AppException(AppResponseCode.INVALID_ACTION, "Không được quyền chỉnh sửa tài khoàn admin");

            user.RoleId = request.RoleId;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<List<MentionUserResponse>> GetMentionableUsersAsync(Guid userId, string? keyword)
        {
            var users = await _userRepository.GetTaggableUsersAsync(userId, keyword);

            var result = _mapper.Map<List<MentionUserResponse>>(users);
            return result;
        }
    }
}
