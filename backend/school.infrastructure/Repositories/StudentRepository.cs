using Microsoft.EntityFrameworkCore;
using School.Application.Abstractions;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public sealed class StudentRepository(AppDbContext db) : IStudentRepository
{
    // ON CONFLICT keeps this safe if two requests for a new student arrive together.
    public Task EnsureExistsAsync(string studentId, DateTimeOffset now, CancellationToken ct) =>
        db.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO students (student_id, created_at) VALUES ({studentId}, {now}) ON CONFLICT (student_id) DO NOTHING",
            ct);
}