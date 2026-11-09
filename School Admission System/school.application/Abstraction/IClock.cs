namespace School.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}