using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.IngredientDetectionDtos;
using SEP490_FTCDHMM_API.Api.Dtos.IngredientDtos;
using SEP490_FTCDHMM_API.Api.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Application.Dtos.UserDietRestriction;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;


namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly IIngredientDetectionService _ingredientDetectionService;
        private readonly IMapper _mapper;

        public IngredientController(IIngredientService ingredientService, IMapper mapper, IIngredientDetectionService ingredientDetectionService)
        {
            _ingredientService = ingredientService;
            _mapper = mapper;
            _ingredientDetectionService = ingredientDetectionService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] Dtos.UserDietRestriction.UserDietRestrictionFilterRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.IngredientDtos.IngredientFilterRequest>(dto);

            var result = await _ingredientService.GetList(appDto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var result = await _ingredientService.GetDetails(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = PermissionPolicies.Ingredient_Create)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateIngredientRequest dto)
        {
            var appDto = _mapper.Map<ApplicationDtos.IngredientDtos.CreateIngredientRequest>(dto);

            await _ingredientService.CreateIngredient(appDto);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = PermissionPolicies.Ingredient_Update)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateIngredientRequest dto, CancellationToken ct)
        {

            var appDto = _mapper.Map<ApplicationDtos.IngredientDtos.UpdateIngredientRequest>(dto);

            await _ingredientService.UpdateIngredient(id, appDto, ct);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = PermissionPolicies.Ingredient_Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _ingredientService.DeleteIngredient(id);
            return Ok(new { message = "Ingredient deleted successfully." });
        }

        [HttpPost("detect-gemini")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> DetectByGemini([FromForm] IngredientDetectionUploadRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.IngredientDetectionDtos.IngredientDetectionUploadRequest>(request);

            var result = await _ingredientDetectionService.DetectIngredientsAsync(appRequest);
            return Ok(result);
        }
    }
}
