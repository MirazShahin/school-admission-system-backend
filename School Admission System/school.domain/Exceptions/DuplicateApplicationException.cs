namespace School.Domain.Exceptions;

public sealed class DuplicateApplicationException()
    : DomainException("DUPLICATE_APPLICATION", "You already have an active application for this class.");