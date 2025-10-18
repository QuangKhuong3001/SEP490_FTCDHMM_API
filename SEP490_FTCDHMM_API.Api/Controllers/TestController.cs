using Microsoft.AspNetCore.Mvc;
using SEP490_FTCDHMM_API.Application.Services.Implementations.SEP490_FTCDHMM_API.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ICacheService _cache;

    public TestController(ICacheService cache)
    {
        _cache = cache;
    }

    [HttpGet("redis")]
    public async Task<IActionResult> TestRedis()
    {
        await _cache.SetAsync("fitfood", "Redis Connected OK!", TimeSpan.FromMinutes(1));
        var value = await _cache.GetAsync<string>("fitfood");
        return Ok(new { redis_value = value });
    }
}