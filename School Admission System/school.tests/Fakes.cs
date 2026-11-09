using School.Application.Abstractions;
using School.Domain.Entities;

namespace School.Tests;

public sealed class FakeClassRepository : IClassRepository
{
    private readonly Dictionary<string, Class> _store = new();
    private readonly Dictionary<string, (int Held, int Confirmed)> _counts = new();

    public Task AddAsync(Class classItem, CancellationToken ct)
    {
        _store[classItem.ClassId] = classItem;
        return Task.CompletedTask;
    }

    public void SetCounts(string classId, int held, int confirmed) =>
        _counts[classId] = (held, confirmed);

    public Task<ClassSeats?> GetWithCountsAsync(string classId, DateTimeOffset now, CancellationToken ct)
    {
        if (!_store.TryGetValue(classId, out var c)) return Task.FromResult<ClassSeats?>(null);
        var (h, cf) = _counts.GetValueOrDefault(classId);
        return Task.FromResult<ClassSeats?>(new ClassSeats(c, h, cf));
    }

    public Task<IReadOnlyList<ClassSeats>> ListWithCountsAsync(DateTimeOffset now, CancellationToken ct)
    {
        IReadOnlyList<ClassSeats> rows = _store.Values
            .Select(c =>
            {
                var (h, cf) = _counts.GetValueOrDefault(c.ClassId);
                return new ClassSeats(c, h, cf);
            })
            .ToList();
        return Task.FromResult(rows);
    }

    public Task<Class?> LockAsync(string classId, CancellationToken ct) =>
        Task.FromResult(_store.GetValueOrDefault(classId));

    public Task<(int Held, int Confirmed)> CountSeatsAsync(string classId, DateTimeOffset now, CancellationToken ct) =>
        Task.FromResult(_counts.GetValueOrDefault(classId));
}

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken ct) =>
        work(ct);
}

public sealed class FixedClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset UtcNow => now;
}