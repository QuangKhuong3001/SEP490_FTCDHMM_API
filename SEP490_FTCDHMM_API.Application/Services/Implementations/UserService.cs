using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Application.Dtos.Common;
using SEP490_FTCDHMM_API.Application.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Interfaces;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

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

        public async Task<LockResultDto> LockCustomerAccount(LockRequestDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != Role.Customer)
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
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                throw new AppException(AppResponseCode.INVALID_ACCOUNT_INFORMATION);

            if (user.Role.Name != Role.Customer)
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
    }
}
