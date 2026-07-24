// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace cCoder.Logging.Exposures.Controllers;

[ApiController]
[Route("Api/Logging/Baseline")]
public sealed class BaselineController(IBaselineExposure baselineExposure) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: baselineExposure.GetBaselinePackages());
}