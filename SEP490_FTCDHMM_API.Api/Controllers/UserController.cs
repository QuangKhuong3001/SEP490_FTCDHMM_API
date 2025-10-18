using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.Common;
using SEP490_FTCDHMM_API.Api.Dtos.UserDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
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

        [Authorize(Policy = PermissionPolicies.CustomerManagement_View)]
        [HttpGet("getCustomers")]
        public async Task<IActionResult> GetCustomerList([FromQuery] PaginationParams dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.Common.PaginationParams>(dto);

            var result = await _userService.GetCustomerList(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.CustomerManagement_Update)]
        [HttpPut("lockCustomer")]
        public async Task<IActionResult> LockCustomer(LockRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.LockRequest>(dto);

            var result = await _userService.LockCustomerAccount(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.CustomerManagement_Update)]
        [HttpPut("unlockCustomer")]
        public async Task<IActionResult> UnLockCustomer(UnlockRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.UnlockRequest>(dto);

            var result = await _userService.UnLockCustomerAccount(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.ModeratorManagement_View)]
        [HttpGet("getModerators")]
        public async Task<IActionResult> GetModeratorList([FromQuery] PaginationParams dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.Common.PaginationParams>(dto);

            var result = await _userService.GetModeratorList(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.ModeratorManagement_Update)]
        [HttpPut("lockModerator")]
        public async Task<IActionResult> LockModerator(LockRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.LockRequest>(dto);

            var result = await _userService.LockModeratorAccount(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.ModeratorManagement_Update)]
        [HttpPut("unlockModerator")]
        public async Task<IActionResult> UnLockModerator(UnlockRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.UnlockRequest>(dto);

            var result = await _userService.UnLockModeratorAccount(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.ModeratorManagement_Create)]
        [HttpPost("createModerator")]
        public async Task<IActionResult> CreateModeratorAccount(CreateModeratorAccountRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.CreateModeratorAccountRequest>(dto);

            var result = await _userService.CreateModeratorAccount(appDto);
            if (!result.Success) return BadRequest(new { success = false, result.Errors });
            return Ok();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var profile = await _userService.GetProfileAsync(userId!, userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? currentUserId = null;
            if (currentUserIdClaim != null && Guid.TryParse(currentUserIdClaim, out var parsedId))
            {
                currentUserId = parsedId;
            }

            var profile = await _userService.GetProfileAsync(userId, currentUserId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest();

            var appDto = _mapper.Map<ApplicationDtos.UserDtos.UpdateProfileRequest>(dto);

            await _userService.UpdateProfileAsync(userId!, appDto);
            return Ok();
        }
        [Authorize]
        [HttpPost("follow/{followeeId}")]
        public async Task<IActionResult> FollowUser(Guid followeeId)
        {
            var followerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.FollowUserAsync(followerId, followeeId);
            return Ok(new { message = "Theo dõi thành công." });
        }

        [Authorize]
        [HttpDelete("unfollow/{followeeId}")]
        public async Task<IActionResult> UnfollowUser(Guid followeeId)
        {
            var followerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.UnfollowUserAsync(followerId, followeeId);
            return Ok(new { message = "Bỏ theo dõi thành công." });
        }

        [Authorize]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userService.GetFollowersAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _userService.GetFollowingAsync(userId);
            return Ok(result);
        }
    }
}
