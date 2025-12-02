using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

namespace SEP490_FTCDHMM_API.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRedis : ControllerBase
    {
        [HttpGet("redis-test")]
        public async Task<IActionResult> Test([FromServices] ICacheService cache)
        {
            await cache.SetAsync("test-key", "Hello Redis", TimeSpan.FromMinutes(1));
            var value = await cache.GetAsync<string>("test-key");
            return Ok(new { value });
        }
    }
}
