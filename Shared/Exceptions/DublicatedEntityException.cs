namespace OrderFlow.Shared.Exceptions;

public class DuplicatedEntityException(string? message) : Exception(message ?? "Entity already exists");