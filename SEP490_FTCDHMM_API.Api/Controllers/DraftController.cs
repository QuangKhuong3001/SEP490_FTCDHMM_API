using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.DraftRecipeDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DraftController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDraftRecipeService _draftRecipeService;

        public DraftController(IMapper mapper, IDraftRecipeService draftRecipeService)
        {
            _mapper = mapper;
            _draftRecipeService = draftRecipeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateDraft(DraftRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.DraftRecipeDtos.DraftRecipeRequest>(request);

            await _draftRecipeService.CreateOrUpdateDraftAsync(userId, appRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetDraft()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _draftRecipeService.GetDraftAsync(userId);
            return Ok(result);
        }
    }
}
