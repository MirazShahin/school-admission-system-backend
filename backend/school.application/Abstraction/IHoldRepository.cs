using School.Domain.Entities;

namespace School.Application.Abstractions;

public sealed record HoldWithClass(Hold Hold, string ClassName);

public interface IHoldRepository
{
    Task AddAsync(Hold hold, CancellationToken ct);
    Task ExpireAllStaleAsync(DateTimeOffset now, CancellationToken ct);
    Task<Hold?> GetAsync(string holdId, CancellationToken ct);
    Task<Hold?> LockAsync(string holdId, CancellationToken ct);
    Task ExpireStaleAsync(string classId, DateTimeOffset now, CancellationToken ct);
    Task<bool> HasLiveHoldAsync(string classId, string studentId, CancellationToken ct);
    Task<IReadOnlyList<HoldWithClass>> ListForStudentAsync(string studentId, CancellationToken ct);

}