using School.Domain.Entities;

namespace School.Application.Abstractions;

public sealed record ClassSeats(Class Class, int Held, int Confirmed);

public interface IClassRepository
{
    Task AddAsync(Class classItem, CancellationToken ct);
    Task<ClassSeats?> GetWithCountsAsync(string classId, DateTimeOffset now, CancellationToken ct);
    Task<IReadOnlyList<ClassSeats>> ListWithCountsAsync(DateTimeOffset now, CancellationToken ct);
    Task<Class?> LockAsync(string classId, CancellationToken ct);
    Task<(int Held, int Confirmed)> CountSeatsAsync(string classId, DateTimeOffset now, CancellationToken ct);
}