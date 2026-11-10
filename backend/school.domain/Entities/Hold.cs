using School.Domain.Exceptions;
using School.Domain.Enums; 

namespace School.Domain.Entities;

public sealed class Hold
{
    private Hold() { }

    public Hold(string holdId, string classId, string studentId, DateTimeOffset now, TimeSpan duration)
    {
        HoldId = holdId;
        ClassId = classId;
        StudentId = studentId;
        Status = HoldStatus.Held;
        ExpiresAt = now + duration;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public string HoldId { get; private set; } = default!;
    public string ClassId { get; private set; } = default!;
    public string StudentId { get; private set; } = default!;
    public HoldStatus Status { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public HoldStatus EffectiveStatus(DateTimeOffset now) =>
        Status == HoldStatus.Held && ExpiresAt <= now ? HoldStatus.Expired : Status;

    public void Confirm(DateTimeOffset now)
    {
        if (Status == HoldStatus.Confirmed) return; 

        if (Status == HoldStatus.Expired || (Status == HoldStatus.Held && ExpiresAt <= now))
            throw new HoldExpiredException();

        if (Status != HoldStatus.Held)
            throw new InvalidHoldStateException($"A {Status} application cannot be confirmed.");

        Status = HoldStatus.Confirmed;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status == HoldStatus.Cancelled) return; 

        if (Status != HoldStatus.Held || ExpiresAt <= now)
            throw new InvalidHoldStateException("Only an active HELD application can be cancelled.");

        Status = HoldStatus.Cancelled;
        UpdatedAt = now;
    }

    public void MarkExpired(DateTimeOffset now)
    {
        if (Status == HoldStatus.Held && ExpiresAt <= now)
        {
            Status = HoldStatus.Expired;
            UpdatedAt = now;
        }
    }
}