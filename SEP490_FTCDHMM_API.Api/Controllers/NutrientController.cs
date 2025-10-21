using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutrientController : ControllerBase
    {
        private readonly INutrientService _nutrientService;

        public NutrientController(INutrientService nutrientService)
        {
            _nutrientService = nutrientService;
        }

        [HttpGet("required")]
        public async Task<IActionResult> GetRequiredNutrient()
        {
            var result = await _nutrientService.GetRequiredNutrientList();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNutrient()
        {
            var result = await _nutrientService.GetAllNutrient();
            return Ok(result);
        }
    }
}
