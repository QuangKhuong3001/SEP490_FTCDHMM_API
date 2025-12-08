using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Api.Dtos.IngredientCategoryDtos;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;
using SEP490_FTCDHMM_API.Domain.Constants;
using ApplicationDtos = SEP490_FTCDHMM_API.Application.Dtos;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IIngredientCategoryService _ingredientCategoryService;

        public IngredientCategoryController(IMapper mapper, IIngredientCategoryService ingredientCategoryService)
        {
            _mapper = mapper;
            _ingredientCategoryService = ingredientCategoryService;
        }

        [HttpGet("getListFilter")]
        public async Task<IActionResult> GetList([FromQuery] IngredientCategoryFilterRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.IngredientCategoryDtos.IngredientCategoryFilterRequest>(request);

            var result = await _ingredientCategoryService.GetAllIngredientCategoriesFilterAsync(appRequest);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] IngredientCategorySearchDropboxRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.IngredientCategoryDtos.IngredientCategorySearchDropboxRequest>(request);

            var result = await _ingredientCategoryService.GetIngredientCategoriesAsync(appRequest);
            return Ok(result);
        }

        [Authorize(Policy = PermissionPolicies.IngredientCategory_Create)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateIngredientCategoryRequest request)
        {
            var appRequest = _mapper.Map<ApplicationDtos.IngredientCategoryDtos.CreateIngredientCategoryRequest>(request);

            await _ingredientCategoryService.CreateIngredientCategoryAsync(appRequest);
            return Ok();
        }

        [Authorize(Policy = PermissionPolicies.IngredientCategory_Delete)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _ingredientCategoryService.DeleteIngredientCategoryAsync(id);
            return Ok();
        }
    }
}
