using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetCustomerListAsync(PaginationParams pagination);

    }
}
