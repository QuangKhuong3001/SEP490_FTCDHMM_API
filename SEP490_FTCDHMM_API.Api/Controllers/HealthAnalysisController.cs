using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthAnalysisController : ControllerBase
    {
        private readonly IRecipeGoalAnalysisService _service;

        public HealthAnalysisController(IRecipeGoalAnalysisService service)
        {
            _service = service;
        }

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyze([FromQuery] Guid recipeId, [FromQuery] Guid healthGoalId)
        {
            var result = await _service.AnalyzeAsync(recipeId, healthGoalId);

            return Ok(result);
        }
    }
}
