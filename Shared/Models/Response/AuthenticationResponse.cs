namespace OrderFlow.Shared.Models.Response;

public class AuthenticationResponse
{
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime AccessTokenExpiration { get; init; }
    public DateTime RefreshTokenExpiration { get; init; }
}