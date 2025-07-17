namespace OrderFlow.Identity.Models.Request;

public record DeviceAuthenticationRequest(string? Token, string? DeviceId);