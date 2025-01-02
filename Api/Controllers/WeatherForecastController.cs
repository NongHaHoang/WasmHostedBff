using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    // GET: api/weatherforecast
    [HttpGet]
    public async Task<ActionResult<string>> GetWeatherForcast()
    {
        return Ok("Nice Weather!");
    }
}