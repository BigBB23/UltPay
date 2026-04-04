
using Microsoft.AspNetCore.Mvc;

namespace UltPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok", app = "UltPay.Api" });
    }
}
