using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetCustomerList(PaginationParams pagination);
        Task<LockResultDto> LockCustomerAccount(LockRequestDto dto);
        Task<UnlockResultDto> UnLockCustomerAccount(UnlockRequestDto dto);
        Task<PagedResult<UserDto>> GetModeratorList(PaginationParams pagination);
        Task<LockResultDto> LockModeratorAccount(LockRequestDto dto);
        Task<UnlockResultDto> UnLockModeratorAccount(UnlockRequestDto dto);
        Task<CreateModeratorAccountResult> CreateModeratorAccount(CreateModeratorAccountDto dto);
        Task<ProfileDto> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
        Task FollowUserAsync(Guid followerId, Guid followeeId);
        Task UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<List<UserFollowResponse>> GetFollowersAsync(Guid userId);
        Task<List<UserFollowResponse>> GetFollowingAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileRequest dto);

    }
}
