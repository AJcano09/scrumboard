using Microsoft.Extensions.Configuration;
using Moq;
using ScrumBoard.Infrastructure.Security;

namespace ScrumBoard.Infrastructure.Tests.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut;

    public PasswordHasherTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Security:Pepper"]).Returns("test-pepper");
        _sut = new PasswordHasher(configMock.Object);
    }

    [Fact]
    public void HashPassword_FormatCorrecto_TresPartesSeparadasPorPunto()
    {
        var hash = _sut.HashPassword("mypassword");

        var parts = hash.Split('.');

        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void VerifyPassword_PasswordCorrecto_DevuelveTrue()
    {
        var hash = _sut.HashPassword("mypassword");

        var result = _sut.VerifyPassword("mypassword", hash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_PasswordIncorrecto_DevuelveFalse()
    {
        var hash = _sut.HashPassword("mypassword");

        var result = _sut.VerifyPassword("wrongpassword", hash);

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_DosHashesDeMismaPassword_SonDistintos()
    {
        var hash1 = _sut.HashPassword("mypassword");
        var hash2 = _sut.HashPassword("mypassword");

        Assert.NotEqual(hash1, hash2);
        Assert.True(_sut.VerifyPassword("mypassword", hash1));
        Assert.True(_sut.VerifyPassword("mypassword", hash2));
    }

    [Fact]
    public void VerifyPassword_HashMalFormado_DevuelveFalse()
    {
        var result = _sut.VerifyPassword("mypassword", "100000.abc");

        Assert.False(result);
    }
}
