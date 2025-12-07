using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos.Mention;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetUserListAsync(UserFilterRequest pagination);
        Task<LockResponse> LockUserAccountAsync(Guid userId, LockRequest dto);
        Task<UnlockResponse> UnLockUserAccountAsync(Guid userId);
        Task<ProfileResponse> GetProfileAsync(string? username, Guid? currentUserId = null);
        Task FollowUserAsync(Guid followerId, Guid followeeId);
        Task UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<IEnumerable<UserInteractionResponse>> GetFollowersAsync(Guid userId);
        Task<IEnumerable<UserInteractionResponse>> GetFollowingAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto);
        Task ChangeActivityLevelAsync(Guid userId, ChangeActivityLevelRequest request);
        Task<ActivityLevel> GetActivityLevelAsync(Guid userId);
        Task ChangeRoleAsync(Guid userId, ChangeRoleRequest request);
        Task<List<MentionUserResponse>> GetMentionableUsersAsync(Guid userId, string? keyword);
    }
}
