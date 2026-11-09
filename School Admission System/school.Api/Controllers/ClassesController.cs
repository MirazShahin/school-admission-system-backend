using Microsoft.AspNetCore.Mvc;
using School.Api.Common;
using School.Application.Classes;
using School.Application.Holds;

namespace School.Api.Controllers;

[ApiController]
[Route("classes")]
public sealed class ClassesController(ClassService classes, HoldService holds) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassCommand cmd, CancellationToken ct) =>
        StatusCode(201, await classes.CreateAsync(cmd, ct));

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct) =>
        Ok(await classes.ListAsync(ct));

    [HttpGet("{classId}")]
    public async Task<IActionResult> Get(string classId, CancellationToken ct) =>
        Ok(await classes.GetAsync(classId, ct));

    [HttpPost("{classId}/holds")]
    public async Task<IActionResult> Apply(string classId, CancellationToken ct)
    {
        var studentId = HttpContext.RequireStudentId();
        return StatusCode(201, await holds.ApplyAsync(classId, studentId, ct));
    }
}