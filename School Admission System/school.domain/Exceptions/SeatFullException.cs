namespace School.Domain.Exceptions;

public sealed class SeatFullException()
    : DomainException("SEAT_FULL", "No seats are left in this class.");