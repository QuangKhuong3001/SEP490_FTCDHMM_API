using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Services.Implementations
{
    internal class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, UserManager<AppUser> userManager, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> GetCustomerListAsync(PaginationParams pagination)
        {
            var (customers, totalCount) = await _userRepository.GetPagedAsync(
        pagination.Page, pagination.PageSize,
        orderBy: q => q.OrderBy(u => u.CreatedAtUtc));

            var result = new List<UserDto>();

            foreach (var user in customers)
            {
                var dto = _mapper.Map<UserDto>(user);

                if (await _userManager.IsInRoleAsync(user, Role.Customer))
                {
                    result.Add(dto);
                }
            }

            return new PagedResult<UserDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

    }
}
