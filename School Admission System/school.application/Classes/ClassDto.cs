using School.Application.Abstractions;

namespace School.Application.Classes;

public sealed record ClassDto(
    string Id,
    string Name,
    int TotalSeats,
    int HeldSeats,
    int ConfirmedSeats,
    int AvailableSeats)
{
    public static ClassDto From(ClassSeats s) => new(
        s.Class.ClassId,
        s.Class.Name,
        s.Class.TotalSeats,
        s.Held,
        s.Confirmed,
        s.Class.TotalSeats - s.Held - s.Confirmed);
}