using Microsoft.Extensions.Options;
using School.Application.Abstractions;
using School.Application.Common;
using School.Domain.Entities;
using School.Domain.Exceptions;

namespace School.Application.Holds;

public sealed class HoldService(
    IClassRepository classes,
    IHoldRepository holds,
    IStudentRepository students,
    IUnitOfWork uow,
    IClock clock,
    IOptions<HoldOptions> options)
{
    public Task<HoldDto> ApplyAsync(string classId, string studentId, CancellationToken ct) =>
        uow.ExecuteInTransactionAsync(async token =>
        {
            var now = clock.UtcNow;

            // 1. Lock the class row. Every apply for this class now runs one at a time.
            var classItem = await classes.LockAsync(classId, token)
                ?? throw new NotFoundException("CLASS_NOT_FOUND", "Class was not found.");

            // 2. Free seats held by expired applications.
            await holds.ExpireStaleAsync(classId, now, token);

            // 3. One active application per student per class.
            if (await holds.HasLiveHoldAsync(classId, studentId, token))
                throw new DuplicateApplicationException();

            // 4. Count under the lock.
            var (held, confirmed) = await classes.CountSeatsAsync(classId, now, token);
            if (held + confirmed >= classItem.TotalSeats)
                throw new SeatFullException();

            // 5. Create the student on first apply, then the hold.
            await students.EnsureExistsAsync(studentId, now, token);

            var hold = new Hold(
                IdGenerator.New("h"),
                classId,
                studentId,
                now,
                TimeSpan.FromSeconds(options.Value.DurationSeconds));

            await holds.AddAsync(hold, token);
            return HoldDto.From(hold, now);
        }, ct);

    public async Task<HoldDto> GetAsync(string holdId, CancellationToken ct)
    {
        var hold = await holds.GetAsync(holdId, ct)
            ?? throw new NotFoundException("HOLD_NOT_FOUND", "Application was not found.");
        return HoldDto.From(hold, clock.UtcNow);
    }

    public Task<HoldDto> ConfirmAsync(string holdId, string studentId, CancellationToken ct) =>
        uow.ExecuteInTransactionAsync(async token =>
        {
            var hold = await holds.LockAsync(holdId, token)
                ?? throw new NotFoundException("HOLD_NOT_FOUND", "Application was not found.");

            // Another student's hold looks the same as a missing one.
            if (hold.StudentId != studentId)
                throw new NotFoundException("HOLD_NOT_FOUND", "Application was not found.");

            var now = clock.UtcNow;
            hold.Confirm(now);
            return HoldDto.From(hold, now);
        }, ct);

    public Task CancelAsync(string holdId, string studentId, CancellationToken ct) =>
        uow.ExecuteInTransactionAsync<bool>(async token =>
        {
            var hold = await holds.LockAsync(holdId, token)
                ?? throw new NotFoundException("HOLD_NOT_FOUND", "Application was not found.");

            if (hold.StudentId != studentId)
                throw new NotFoundException("HOLD_NOT_FOUND", "Application was not found.");

            hold.Cancel(clock.UtcNow);
            return true;
        }, ct);

    public async Task<IReadOnlyList<HoldSummaryDto>> ListForStudentAsync(string studentId, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var rows = await holds.ListForStudentAsync(studentId, ct);
        return rows.Select(r => HoldSummaryDto.From(r, now)).ToList();
    }
}