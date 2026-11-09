using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Tests;

public sealed class HoldExpiryTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 9, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void MarkExpired_flips_a_past_due_held_hold()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.MarkExpired(Now.AddMinutes(11));

        Assert.Equal(HoldStatus.Expired, hold.Status);
    }

    [Fact]
    public void MarkExpired_leaves_a_live_hold_alone()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));

        hold.MarkExpired(Now.AddMinutes(5));

        Assert.Equal(HoldStatus.Held, hold.Status);
    }

    [Fact]
    public void MarkExpired_does_not_touch_a_confirmed_hold()
    {
        var hold = new Hold("h_1", "c_1", "20152691234567890", Now, TimeSpan.FromMinutes(10));
        hold.Confirm(Now.AddMinutes(1));

        hold.MarkExpired(Now.AddMinutes(20));

        Assert.Equal(HoldStatus.Confirmed, hold.Status);
    }
}