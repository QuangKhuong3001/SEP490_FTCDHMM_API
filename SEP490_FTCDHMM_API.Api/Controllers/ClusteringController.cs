using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.ClusterInterfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClusteringController : ControllerBase
    {
        private readonly IKMeansAppService _kMeansAppService;

        public ClusteringController(IKMeansAppService kMeansAppService)
        {
            _kMeansAppService = kMeansAppService;
        }

        [HttpPost("compute")]
        public async Task<IActionResult> Compute([FromQuery] int k = 2)
        {
            var result = await _kMeansAppService.ComputeAsync(k);
            return Ok(result);
        }
    }
}