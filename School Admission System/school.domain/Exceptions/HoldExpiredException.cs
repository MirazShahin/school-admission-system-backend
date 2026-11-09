namespace School.Domain.Exceptions;

public sealed class HoldExpiredException()
    : DomainException("HOLD_EXPIRED", "This application has expired.");