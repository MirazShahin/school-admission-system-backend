namespace School.Domain.Exceptions;

public class DomainException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

public sealed class InvalidHoldStateException(string message)
    : DomainException("INVALID_HOLD_STATE", message);

public sealed class NotFoundException(string code, string message)
    : DomainException(code, message);

public sealed class ValidationFailedException(string code, string message)
    : DomainException(code, message);