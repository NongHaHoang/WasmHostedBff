using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    // GET: api/weatherforecast
    [HttpGet]
    public async Task<ActionResult<string>> GetWeatherForcast()
    {
        logger.LogInformation("Weather forecast requested");

        return Ok("Nice Weather!");
    }
}