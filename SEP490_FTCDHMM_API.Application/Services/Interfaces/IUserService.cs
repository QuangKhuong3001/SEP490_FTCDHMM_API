using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetCustomerList(PaginationParams pagination);
        Task<LockResponse> LockCustomerAccount(LockRequest dto);
        Task<UnlockResponse> UnLockCustomerAccount(UnlockRequest dto);
        Task<PagedResult<UserResponse>> GetModeratorList(PaginationParams pagination);
        Task<LockResponse> LockModeratorAccount(LockRequest dto);
        Task<UnlockResponse> UnLockModeratorAccount(UnlockRequest dto);
        Task<CreateModeratorAccountResponse> CreateModeratorAccount(CreateModeratorAccountRequest dto);
        Task<ProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task FollowUserAsync(Guid followerId, Guid followeeId);
        Task UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<List<UserResponse>> GetFollowersAsync(Guid userId);
        Task<List<UserResponse>> GetFollowingAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto);

    }
}
