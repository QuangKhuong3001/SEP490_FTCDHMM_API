using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IDistributedCache _cache;
    public TestController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet("redis")]
    public async Task<IActionResult> TestRedis()
    {
        await _cache.SetStringAsync("fitfood", "Redis Connected OK!", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });

        var value = await _cache.GetStringAsync("fitfood");
        return Ok(new { redis_value = value });
    }
}
