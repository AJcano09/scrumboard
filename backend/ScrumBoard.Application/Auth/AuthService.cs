using ScrumBoard.Application.Ports;

namespace ScrumBoard.Application.Auth;

public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user is null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var token = tokenService.GenerateToken(user.Id, user.Email);
        return new LoginResponse(token, user.Name, user.Email);
    }
}