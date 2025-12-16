using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

namespace SEP490_FTCDHMM_API.API.Controllers
{
    [ApiController]
    [Route("api/redis")]
    public class RedisTestController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public RedisTestController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            var key = "redis:test:ping";
            var value = new
            {
                Message = "Redis is working",
                Time = DateTime.UtcNow
            };

            await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(5));
            var result = await _cacheService.GetAsync<object>(key);

            if (result == null)
                return StatusCode(500, "Redis connected but failed to read data");

            return Ok(new
            {
                Status = "OK",
                Data = result
            });
        }
    }
}
