using Microsoft.EntityFrameworkCore;
using School.Application.Abstractions;
using School.Domain.Entities;
using School.Domain.Enums;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public sealed class HoldRepository(AppDbContext db) : IHoldRepository
{
    public Task AddAsync(Hold hold, CancellationToken ct)
    {
        db.Holds.Add(hold);
        return Task.CompletedTask;
    }

    public Task<Hold?> GetAsync(string holdId, CancellationToken ct) =>
        db.Holds.AsNoTracking().FirstOrDefaultAsync(h => h.HoldId == holdId, ct);

    public Task<Hold?> LockAsync(string holdId, CancellationToken ct) =>
        db.Holds
            .FromSqlInterpolated($"SELECT * FROM holds WHERE hold_id = {holdId} FOR UPDATE")
            .FirstOrDefaultAsync(ct);

    // Marks old HELD rows as EXPIRED so the one-live-application index does not block a re-apply.
    public Task ExpireStaleAsync(string classId, DateTimeOffset now, CancellationToken ct) =>
        db.Holds
            .Where(h => h.ClassId == classId && h.Status == HoldStatus.Held && h.ExpiresAt <= now)
            .ExecuteUpdateAsync(s => s
                .SetProperty(h => h.Status, HoldStatus.Expired)
                .SetProperty(h => h.UpdatedAt, now), ct);

    public Task<bool> HasLiveHoldAsync(string classId, string studentId, CancellationToken ct) =>
        db.Holds.AnyAsync(h =>
            h.ClassId == classId &&
            h.StudentId == studentId &&
            (h.Status == HoldStatus.Held || h.Status == HoldStatus.Confirmed), ct);

    public async Task<IReadOnlyList<HoldWithClass>> ListForStudentAsync(string studentId, CancellationToken ct) =>
        await (from h in db.Holds.AsNoTracking()
               join c in db.Classes.AsNoTracking() on h.ClassId equals c.ClassId
               where h.StudentId == studentId
               orderby h.CreatedAt descending
               select new HoldWithClass(h, c.Name))
              .ToListAsync(ct);
}