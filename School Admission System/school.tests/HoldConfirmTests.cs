using School.Domain.Entities;
using School.Domain.Enums;
using School.Domain.Exceptions;

namespace School.Tests;

public sealed class HoldConfirmTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Confirm_moves_a_live_held_hold_to_confirmed()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.Confirm(Now.AddMinutes(1));

        Assert.Equal(HoldStatus.Confirmed, hold.Status);
    }

    [Fact]
    public void Confirm_twice_is_idempotent()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.Confirm(Now.AddMinutes(1));
        hold.Confirm(Now.AddMinutes(2));

        Assert.Equal(HoldStatus.Confirmed, hold.Status);
    }

    [Fact]
    public void Confirm_after_expiry_throws_HOLD_EXPIRED()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        var ex = Assert.Throws<HoldExpiredException>(() => hold.Confirm(Now.AddMinutes(11)));

        Assert.Equal("HOLD_EXPIRED", ex.Code);
        Assert.Equal(HoldStatus.Held, hold.Status);
    }

    [Fact]
    public void Confirm_a_cancelled_hold_throws_INVALID_HOLD_STATE()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));
        hold.Cancel(Now.AddMinutes(1));

        var ex = Assert.Throws<InvalidHoldStateException>(() => hold.Confirm(Now.AddMinutes(2)));

        Assert.Equal("INVALID_HOLD_STATE", ex.Code);
    }

    [Fact]
    public void Effective_status_reports_expired_for_past_held_hold()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        Assert.Equal(HoldStatus.Expired, hold.EffectiveStatus(Now.AddMinutes(11)));
        Assert.Equal(HoldStatus.Held, hold.EffectiveStatus(Now.AddMinutes(5)));
    }
}