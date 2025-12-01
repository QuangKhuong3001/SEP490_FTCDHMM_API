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
        public async Task<IActionResult> Create(DraftRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.DraftRecipeDtos.DraftRecipeRequest>(request);

            await _draftRecipeService.CreateDraftAsync(userId, appRequest);
            return Ok();
        }

        [HttpPut("{draftId:guid}")]
        public async Task<IActionResult> Update(Guid draftId, DraftRecipeRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appRequest = _mapper.Map<ApplicationDtos.DraftRecipeDtos.DraftRecipeRequest>(request);

            await _draftRecipeService.UpdateDraftAsync(userId, draftId, appRequest);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _draftRecipeService.GetDraftsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{draftId:guid}")]
        public async Task<IActionResult> GetById(Guid draftId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _draftRecipeService.GetDraftByIdAsync(userId, draftId);
            return Ok(result);
        }
    }
}
