using Microsoft.AspNetCore.Mvc;
using School.Api.Common;
using School.Application.Holds;

namespace School.Api.Controllers;

[ApiController]
[Route("me")]
public sealed class MeController(HoldService holds) : ControllerBase
{
    [HttpGet("holds")]
    public async Task<IActionResult> MyHolds(CancellationToken ct)
    {
        var studentId = HttpContext.RequireStudentId();
        return Ok(await holds.ListForStudentAsync(studentId, ct));
    }
}