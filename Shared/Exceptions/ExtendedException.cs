namespace OrderFlow.Shared.Exceptions;

public class EntityNotFoundException(string? message) : Exception(message ?? "Entity not found");
public class DuplicatedEntityException(string? message) : Exception(message ?? "Entity already exists");
