using School.Application.Abstractions;
using School.Application.Common;
using School.Domain.Entities;
using School.Domain.Exceptions;

namespace School.Application.Classes;

public sealed class ClassService(IClassRepository classes, IUnitOfWork uow, IClock clock)
{
    public Task<ClassDto> CreateAsync(CreateClassCommand cmd, CancellationToken ct) =>
        uow.ExecuteInTransactionAsync(async token =>
        {
            var classItem = new Class(IdGenerator.New("c"), cmd.Name, cmd.TotalSeats, clock.UtcNow);
            await classes.AddAsync(classItem, token);
            return ClassDto.From(new ClassSeats(classItem, 0, 0));
        }, ct);

    public async Task<IReadOnlyList<ClassDto>> ListAsync(CancellationToken ct)
    {
        var rows = await classes.ListWithCountsAsync(clock.UtcNow, ct);
        return rows.Select(ClassDto.From).ToList();
    }

    public async Task<ClassDto> GetAsync(string classId, CancellationToken ct)
    {
        var row = await classes.GetWithCountsAsync(classId, clock.UtcNow, ct)
            ?? throw new NotFoundException("CLASS_NOT_FOUND", "Class was not found.");
        return ClassDto.From(row);
    }
}