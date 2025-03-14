namespace OrderFlow.Shared.Exceptions;

public class EntityNotFoundException(string? message) : Exception(message ?? "Entity not found");

