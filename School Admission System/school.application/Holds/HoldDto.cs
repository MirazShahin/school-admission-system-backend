using School.Application.Abstractions;
using School.Domain.Entities;
using School.Domain.Enums;

namespace School.Application.Holds;

public sealed record HoldDto(
    string HoldId,
    string ClassId,
    string StudentId,
    string Status,
    DateTimeOffset ExpiresAt)
{
    public static HoldDto From(Hold h, DateTimeOffset now) => new(
        h.HoldId,
        h.ClassId,
        h.StudentId,
        h.EffectiveStatus(now).ToDb(),
        h.ExpiresAt);
}

public sealed record HoldSummaryDto(
    string HoldId,
    string ClassId,
    string ClassName,
    string Status,
    DateTimeOffset ExpiresAt)
{
    public static HoldSummaryDto From(HoldWithClass row, DateTimeOffset now) => new(
        row.Hold.HoldId,
        row.Hold.ClassId,
        row.ClassName,
        row.Hold.EffectiveStatus(now).ToDb(),
        row.Hold.ExpiresAt);
}