using School.Domain.Entities;
using School.Domain.Exceptions;

namespace School.Api.Common;

public static class StudentIdExtensions
{
    public static string RequireStudentId(this HttpContext ctx)
    {
        var value = ctx.Request.Headers["X-Student-Id"].FirstOrDefault();
        if (!Student.IsValidStudentId(value))
            throw new ValidationFailedException("INVALID_STUDENT_ID", "X-Student-Id must be exactly 17 digits.");
        return value!;
    }
}