using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos.Mention;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetCustomerList(UserFilterRequest pagination);
        Task<LockResponse> LockCustomerAccount(Guid userId, LockRequest dto);
        Task<UnlockResponse> UnLockCustomerAccount(Guid userId);
        Task<PagedResult<UserResponse>> GetModeratorList(UserFilterRequest pagination);
        Task<LockResponse> LockModeratorAccount(Guid userId, LockRequest dto);
        Task<UnlockResponse> UnLockModeratorAccount(Guid userId);
        Task<CreateModeratorAccountResponse> CreateModeratorAccount(CreateModeratorAccountRequest dto);
        Task<ProfileResponse> GetProfileAsync(Guid userId, Guid? currentUserId = null);
        Task FollowUserAsync(Guid followerId, Guid followeeId);
        Task UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<IEnumerable<UserInteractionResponse>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<UserInteractionResponse>> GetFollowingAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto);
        Task ChangeActivityLevel(Guid userId, ChangeActivityLevelRequest request);
        Task<ActivityLevel> GetActivityLevel(Guid userId);
        Task ChangeRole(Guid userId, ChangeRoleRequest request);
        Task<IEnumerable<MentionUserResponse>> GetMentionableUsersAsync(Guid userId, string? keyword);
    }
}
