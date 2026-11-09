using Microsoft.EntityFrameworkCore;
using School.Application.Abstractions;
using School.Application.Classes;
using School.Application.Holds;
using School.Domain.Entities;
using School.Domain.Exceptions;
using School.Infrastructure.Persistence;
using School.Infrastructure.Repositories;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.Options;

namespace School.Tests;

public sealed class HoldConcurrencyTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("school_admission")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private string _connectionString = default!;

    public async Task InitializeAsync()
    {
        await _pg.StartAsync();
        _connectionString = _pg.GetConnectionString();

        await using var db = CreateDb();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _pg.DisposeAsync();

    private AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;
        return new AppDbContext(options);
    }

    private (HoldService holds, ClassService classes) CreateServices()
    {
        // Each call gets its own DbContext, so parallel requests do not share a connection.
        var db = CreateDb();
        var clock = new SystemClockForTests();
        var uow = new UnitOfWork(db);
        var classRepo = new ClassRepository(db);
        var holdRepo = new HoldRepository(db);
        var studentRepo = new StudentRepository(db);
        var options = Options.Create(new HoldOptions { DurationSeconds = 600 });

        return (
            new HoldService(classRepo, holdRepo, studentRepo, uow, clock, options),
            new ClassService(classRepo, uow, clock));
    }

    [Fact]
    public async Task Parallel_applications_never_exceed_total_seats()
    {
        var (_, classes) = CreateServices();
        var created = await classes.CreateAsync(new CreateClassCommand("Class 6 - Morning Shift", 10), default);

        var tasks = Enumerable.Range(0, 50).Select(async i =>
        {
            var (holds, _) = CreateServices();
            var studentId = (20152691234500000L + i).ToString();
            try
            {
                await holds.ApplyAsync(created.Id, studentId, default);
                return true;
            }
            catch (SeatFullException)
            {
                return false;
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.Equal(10, results.Count(r => r));
        Assert.Equal(40, results.Count(r => !r));

        await using var check = CreateDb();
        var held = await check.Holds.CountAsync(h =>
            h.ClassId == created.Id && h.Status == School.Domain.Enums.HoldStatus.Held);
        Assert.Equal(10, held);
    }

    [Fact]
    public async Task Same_student_applying_in_parallel_gets_only_one_hold()
    {
        var (_, classes) = CreateServices();
        var created = await classes.CreateAsync(new CreateClassCommand("Class 7", 10), default);
        const string studentId = "20152691234567890";

        var tasks = Enumerable.Range(0, 20).Select(async _ =>
        {
            var (holds, _) = CreateServices();
            try
            {
                await holds.ApplyAsync(created.Id, studentId, default);
                return true;
            }
            catch (DuplicateApplicationException)
            {
                return false;
            }
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(r => r));
    }
}

public sealed class SystemClockForTests : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}