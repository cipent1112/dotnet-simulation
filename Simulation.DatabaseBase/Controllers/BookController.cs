using Microsoft.AspNetCore.Mvc;

namespace Simulation.DatabaseBase.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}