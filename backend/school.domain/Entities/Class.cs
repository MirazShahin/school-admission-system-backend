using School.Domain.Exceptions;

namespace School.Domain.Entities;

public sealed class Class
{
    public const int MaxNameLength = 120;

    private Class() { }

    public Class(string classId, string name, int totalSeats, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationFailedException("NAME_REQUIRED", "Class name is required.");
        if (name.Trim().Length > MaxNameLength)
            throw new ValidationFailedException("NAME_TOO_LONG", $"Class name must be at most {MaxNameLength} characters.");
        if (totalSeats < 1)
            throw new ValidationFailedException("INVALID_SEATS", "totalSeats must be at least 1.");

        ClassId = classId;
        Name = name.Trim();
        TotalSeats = totalSeats;
        CreatedAt = createdAt;
    }

    public string ClassId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int TotalSeats { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}