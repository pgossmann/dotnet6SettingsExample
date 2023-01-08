using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AppSettingsTest.Controllers;

[ApiController]
[Route("settings")]
public class SettingsController : ControllerBase
{

    private readonly ILogger<SettingsController> _logger;
    private readonly Settings _settings;

    // settings are injected with the IOptions<Settings>
    public SettingsController(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

    [Route("get")]
    [HttpGet]
    public string Get()
    {
        return JsonSerializer.Serialize(_settings);
    }
}
