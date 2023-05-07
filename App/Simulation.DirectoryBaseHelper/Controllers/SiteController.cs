using Microsoft.AspNetCore.Mvc;
using Simulation.DirectoryBaseHelper.Common.Helper;

namespace Simulation.DirectoryBaseHelper.Controllers;

[ApiController]
[Route("~/")]
public class SiteController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    public SiteController(IWebHostEnvironment env) => _env = env;

    [HttpGet]
    public object Get()
    {
        return new
        {
            _env.ContentRootPath,
            _env.WebRootPath,
            DirectoryHelperContentRootPath = DirectoryHelper.ContentRootPath,
            DirectoryHelperWebRootPath = DirectoryHelper.WebRootPath,
            DirectoryHelperConfigRootPath = DirectoryHelper.ConfigRootPath,
            DirectoryHelperUploadRootPath = DirectoryHelper.UploadRootPath
        };
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Post([FromForm] IFormFile file)
    {
        var filePath = Path.Combine(DirectoryHelper.UploadRootPath, file.FileName);
        var stream = file.OpenReadStream();
        await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
        await stream.CopyToAsync(fileStream);
        await stream.FlushAsync();
        await stream.DisposeAsync();

        return Ok("Upload success");
    }
}