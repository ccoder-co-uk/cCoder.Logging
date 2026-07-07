using cCoder.Logging.Exposures.Setup;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Logging.Exposures.Controllers;

[ApiController]
[Route("Api/Logging/Baseline")]
public sealed class BaselineController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(UIBaseline.Packages);
}
