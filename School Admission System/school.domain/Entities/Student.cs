namespace School.Domain.Entities;

public sealed class Student
{
    private Student() { }

    public Student(string studentId, DateTimeOffset createdAt)
    {
        StudentId = studentId;
        CreatedAt = createdAt;
    }

    public string StudentId { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    public static bool IsValidStudentId(string? value) =>
        value is { Length: 17 } && value.All(char.IsAsciiDigit);
}