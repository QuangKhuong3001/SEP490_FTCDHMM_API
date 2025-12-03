using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize(Policy = PermissionPolicies.UserManagement_View)]
        [HttpGet()]
        public async Task<IActionResult> GetUserList([FromQuery] UserFilterRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.UserFilterRequest>(dto);

            var result = await _userService.GetUserList(appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.UserManagement_Update)]
        [HttpPut("{userId:guid}/lock")]
        public async Task<IActionResult> LockUser(Guid userId, LockRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.UserDtos.LockRequest>(dto);

            var result = await _userService.LockUserAccount(userId, appDto);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.UserManagement_Update)]
        [HttpPut("{userId:guid}/unlock")]
        public async Task<IActionResult> UnLockModerator(Guid userId)
        {
            var result = await _userService.UnLockUserAccount(userId);
            return Ok(result);
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
            return Ok();
        }

        [Authorize]
        [HttpDelete("unfollow/{followeeId}")]
        public async Task<IActionResult> UnfollowUser(Guid followeeId)
        {
            var followerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _userService.UnfollowUserAsync(followerId, followeeId);
            return Ok();
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

        [Authorize]
        [HttpGet("activity-level")]
        public async Task<IActionResult> GetActivityLevel()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _userService.GetActivityLevel(userId);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("activity-level")]
        public async Task<IActionResult> ChangeActivityLevel(ChangeActivityLevelRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.UserDtos.ChangeActivityLevelRequest>(request);

            await _userService.ChangeActivityLevel(userId, appRequest);
            return Ok();
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("{userId:guid}/roles")]
        public async Task<IActionResult> AssignRole(Guid userId, ChangeRoleRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.UserDtos.ChangeRoleRequest>(request);

            await _userService.ChangeRole(userId, appRequest);

            return Ok();
        }

        [Authorize]
        [HttpGet("taggable-users")]
        public async Task<IActionResult> GetTaggableUsers([FromQuery] string? keyword)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var users = await _userService.GetMentionableUsersAsync(userId, keyword);

            return Ok(users);
        }
    }
}
