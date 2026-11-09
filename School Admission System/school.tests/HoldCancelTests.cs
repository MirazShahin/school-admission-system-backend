using School.Domain.Entities;
using School.Domain.Enums;
using School.Domain.Exceptions;

namespace School.Tests;

public sealed class HoldCancelTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Cancel_moves_a_live_held_hold_to_cancelled()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.Cancel(Now.AddMinutes(1));

        Assert.Equal(HoldStatus.Cancelled, hold.Status);
    }

    [Fact]
    public void Cancel_twice_is_idempotent()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.Cancel(Now.AddMinutes(1));
        hold.Cancel(Now.AddMinutes(2));

        Assert.Equal(HoldStatus.Cancelled, hold.Status);
    }

    [Fact]
    public void Cancel_a_confirmed_hold_throws_INVALID_HOLD_STATE()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));
        hold.Confirm(Now.AddMinutes(1));

        var ex = Assert.Throws<InvalidHoldStateException>(() => hold.Cancel(Now.AddMinutes(2)));

        Assert.Equal("INVALID_HOLD_STATE", ex.Code);
        Assert.Equal(HoldStatus.Confirmed, hold.Status);
    }

    [Fact]
    public void Cancel_an_expired_hold_throws_INVALID_HOLD_STATE()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        var ex = Assert.Throws<InvalidHoldStateException>(() => hold.Cancel(Now.AddMinutes(11)));

        Assert.Equal("INVALID_HOLD_STATE", ex.Code);
    }
}