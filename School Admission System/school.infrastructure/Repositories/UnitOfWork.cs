using Microsoft.EntityFrameworkCore;
using Npgsql;
using School.Application.Abstractions;
using School.Domain.Exceptions;
using School.Infrastructure.Persistence;

namespace School.Infrastructure.Repositories;

public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var result = await work(ct);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
        { ConstraintName: "holds_one_live_per_student_class_uq" })
        {
            throw new DuplicateApplicationException();
        }

        await tx.CommitAsync(ct);
        return result;
    }
}