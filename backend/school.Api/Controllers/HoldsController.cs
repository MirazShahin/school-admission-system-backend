using Microsoft.AspNetCore.Mvc;
using School.Api.Common;
using School.Application.Holds;

namespace School.Api.Controllers;

[ApiController]
[Route("holds")]
public sealed class HoldsController(HoldService holds) : ControllerBase
{
    [HttpGet("{holdId}")]
    public async Task<IActionResult> Get(string holdId, CancellationToken ct) =>
        Ok(await holds.GetAsync(holdId, ct));

    [HttpPost("{holdId}/confirm")]
    public async Task<IActionResult> Confirm(string holdId, CancellationToken ct)
    {
        var studentId = HttpContext.RequireStudentId();
        return Ok(await holds.ConfirmAsync(holdId, studentId, ct));
    }

    [HttpDelete("{holdId}")]
    public async Task<IActionResult> Cancel(string holdId, CancellationToken ct)
    {
        var studentId = HttpContext.RequireStudentId();
        await holds.CancelAsync(holdId, studentId, ct);
        return NoContent();
    }
}