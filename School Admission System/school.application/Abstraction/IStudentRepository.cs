namespace School.Application.Abstractions;

public interface IStudentRepository
{
    Task EnsureExistsAsync(string studentId, DateTimeOffset now, CancellationToken ct);
}