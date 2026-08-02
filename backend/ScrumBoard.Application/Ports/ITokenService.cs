namespace ScrumBoard.Application.Ports;

public interface ITokenService
{
    string GenerateToken(Guid userId, string email);
}