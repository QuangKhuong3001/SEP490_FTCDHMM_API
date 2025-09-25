using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = Role.Moderator)]
        public async Task<IActionResult> GetCustomerList(PaginationParams dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.Common.PaginationParams>(dto);

            var result = await _userService.GetCustomerListAsync(appDto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = Role.Moderator)]
        public async Task<IActionResult> LockCustomer(LockRequestDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.LockRequestDto>(dto);

            var result = await _userService.LockCustomerAccount(appDto);
            return Ok(new
            {
                message = $"User {result.Email} locked until {result.LockoutEnd:yyyy-MM-dd HH:mm:ss} UTC"
            });
        }

        [HttpPut]
        [Authorize(Roles = Role.Moderator)]
        public async Task<IActionResult> UnLockCustomer(UnlockRequestDto dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.UnlockRequestDto>(dto);

            var result = await _userService.UnLockCustomerAccount(appDto);
            return Ok(new
            {
                message = $"User {result.Email} unlocked"
            });
        }

    }
}
