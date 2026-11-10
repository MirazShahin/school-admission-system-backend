using School.Application.Abstractions;
using School.Application.Classes;
using School.Domain.Entities;
using School.Domain.Exceptions;

namespace School.Tests;

public sealed class ClassServiceTests
{
    private readonly FakeClassRepository _classes = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FixedClock _clock = new(new DateTimeOffset(2026, 11, 9, 10, 0, 0, TimeSpan.Zero));
    private ClassService CreateService() => new(_classes, _uow, _clock);

    [Fact]
    public async Task Create_with_valid_input_returns_class_with_full_availability()
    {
        var service = CreateService();

        var result = await service.CreateAsync(new CreateClassCommand("Class 6 - Morning Shift", 10), default);

        Assert.StartsWith("c_", result.Id);
        Assert.Equal("Class 6 - Morning Shift", result.Name);
        Assert.Equal(10, result.TotalSeats);
        Assert.Equal(10, result.AvailableSeats);
    }

    [Fact]
    public async Task Create_with_empty_name_throws_NAME_REQUIRED()
    {
        var service = CreateService();

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() =>
            service.CreateAsync(new CreateClassCommand("   ", 10), default));

        Assert.Equal("NAME_REQUIRED", ex.Code);
    }

    [Fact]
    public async Task Create_with_zero_seats_throws_INVALID_SEATS()
    {
        var service = CreateService();

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() =>
            service.CreateAsync(new CreateClassCommand("Class 6", 0), default));

        Assert.Equal("INVALID_SEATS", ex.Code);
    }

    [Fact]
    public async Task Get_unknown_class_throws_CLASS_NOT_FOUND()
    {
        var service = CreateService();

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetAsync("c_missing", default));

        Assert.Equal("CLASS_NOT_FOUND", ex.Code);
    }

    [Fact]
    public async Task List_returns_available_seats_as_total_minus_held_and_confirmed()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateClassCommand("Class 7", 10), default);
        _classes.SetCounts(created.Id, held: 3, confirmed: 2);

        var list = await service.ListAsync(default);

        var row = Assert.Single(list);
        Assert.Equal(3, row.HeldSeats);
        Assert.Equal(2, row.ConfirmedSeats);
        Assert.Equal(5, row.AvailableSeats);
    }
}