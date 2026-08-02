namespace ScrumBoard.Application.Auth;

public record LoginResponse(string Token, string Name, string Email);