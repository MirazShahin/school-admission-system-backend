using Microsoft.EntityFrameworkCore;
using School.Application.Abstractions;
using School.Domain.Entities;
using School.Domain.Enums;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public sealed class ClassRepository(AppDbContext db) : IClassRepository
{
    public Task AddAsync(Class classItem, CancellationToken ct)
    {
        db.Classes.Add(classItem);
        return Task.CompletedTask;
    }

    public async Task<ClassSeats?> GetWithCountsAsync(string classId, DateTimeOffset now, CancellationToken ct)
    {
        var c = await db.Classes.AsNoTracking().FirstOrDefaultAsync(x => x.ClassId == classId, ct);
        if (c is null) return null;

        var (held, confirmed) = await CountSeatsAsync(classId, now, ct);
        return new ClassSeats(c, held, confirmed);
    }

    // One query for all classes, one GROUP BY for all counts. No N+1.
    public async Task<IReadOnlyList<ClassSeats>> ListWithCountsAsync(DateTimeOffset now, CancellationToken ct)
    {
        var classes = await db.Classes.AsNoTracking().OrderBy(c => c.CreatedAt).ToListAsync(ct);

        var counts = await db.Holds.AsNoTracking()
            .GroupBy(h => h.ClassId)
            .Select(g => new
            {
                ClassId = g.Key,
                Held = g.Sum(h => h.Status == HoldStatus.Held && h.ExpiresAt > now ? 1 : 0),
                Confirmed = g.Sum(h => h.Status == HoldStatus.Confirmed ? 1 : 0)
            })
            .ToDictionaryAsync(x => x.ClassId, ct);

        return classes
            .Select(c =>
            {
                counts.TryGetValue(c.ClassId, out var n);
                return new ClassSeats(c, n?.Held ?? 0, n?.Confirmed ?? 0);
            })
            .ToList();
    }

    // Locks the class row. Concurrent applies for the same class wait here.
    public Task<Class?> LockAsync(string classId, CancellationToken ct) =>
        db.Classes
            .FromSqlInterpolated($"SELECT * FROM classes WHERE class_id = {classId} FOR UPDATE")
            .FirstOrDefaultAsync(ct);

    public async Task<(int Held, int Confirmed)> CountSeatsAsync(string classId, DateTimeOffset now, CancellationToken ct)
    {
        var held = await db.Holds.CountAsync(h =>
            h.ClassId == classId && h.Status == HoldStatus.Held && h.ExpiresAt > now, ct);
        var confirmed = await db.Holds.CountAsync(h =>
            h.ClassId == classId && h.Status == HoldStatus.Confirmed, ct);
        return (held, confirmed);
    }
}