namespace OrderFlow.Shared.Exceptions;

public class AccessDeniedException(string? message = null) : Exception(message);